using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Linq;

public enum BattleState
{
    StarBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    Busy,
    PartySelectScreen,
    ItemSelectScreen,
    ForgetMovement,
    LoseTurn,
    FinishBattle
}

public enum BattleType
{
    wildPokemon,
    trainer,
    Leader
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerunit;

    [SerializeField] private BattleUnit enmeyUnit;


    [SerializeField] private BattleDialogBox batteDialogBox;

    [SerializeField] private PartyHUD partiHUD;

    [SerializeField] private SelectionMovementUI selectMoveUI;

    [SerializeField] GameObject pokeball;

    public BattleState state;
    public BattleType type;

    
    private float timeSinceLastClick;
    [SerializeField]private float timeBetweenClicks = 1.0f;

    private int currentSelecteAction;
    private int currentSelectedMovement;
    private int currentSelectedPokemon;

    public event Action<bool> OnBattleFinish;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;

    private int escapeAttemps;//intentos de escape

    private MoveBase moveToLearn;

    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        type = BattleType.wildPokemon;
        escapeAttemps = 0;
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;

        StartCoroutine(SetupBattle());
    }

    public void HandleStartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty, bool isleader)
    {
        type = (isleader ? BattleType.Leader : BattleType.trainer);
        //el resto de batalla contra NPc
    }

    //---------- inicia y fin de la batalla ------------
    public IEnumerator SetupBattle()
    {
        
        state = BattleState.StarBattle;

        //------------- player ---------------
        playerunit.SetupPokemon(playerParty.GetFirstNonFaintedPokemon());
        batteDialogBox.SetPokemonMovements(playerunit.Pokemon.Moves);
        //----------------------------

        //----------- enemy -----------------
        enmeyUnit.SetupPokemon(wildPokemon);
        //------------------------------------------

        partiHUD.InitPartyHUD();//inicializar la parti de batalla

        yield return batteDialogBox.SetDialog($"Un {enmeyUnit.Pokemon.Base.Name} salvaje apareció.");

        if (enmeyUnit.Pokemon.Speed > playerunit.Pokemon.Speed)
        {
            batteDialogBox.ToggleDialogText(true);
            batteDialogBox.ToggleActions(false);
            batteDialogBox.ToggleMovements(false);
            yield return batteDialogBox.SetDialog("El enemigo ataca primero");
            yield return PerformEnemyMovement();
        }
        else
        {
            PlayerActionSelection();
        }
    }

    public void BattleFinish(bool playerHasWon)
    {
        state = BattleState.FinishBattle;

        OnBattleFinish(playerHasWon);
    }
    //---------------------------------------------------

    //------ Menus de UI - Todas la acciones ------------
    private void PlayerActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(batteDialogBox.SetDialog("Seleciona una action"));
        batteDialogBox.ToggleDialogText(true);
        batteDialogBox.ToggleActions(true);
        batteDialogBox.ToggleMovements(false);
        currentSelecteAction = 0;
        batteDialogBox.SelectAction(currentSelecteAction);
    }

    private void PlayerMovementSelection()
    {
        state = BattleState.MovementSelection;
        batteDialogBox.ToggleDialogText(false);
        batteDialogBox.ToggleActions(false);
        batteDialogBox.ToggleMovements(true);
        currentSelectedMovement = 0;
        batteDialogBox.SelectMovement(currentSelectedMovement,playerunit.Pokemon.Moves[currentSelectedMovement]);
    }

    private void OpenPartySelectionScreen()
    {
        //print("Abrir la pantalla para seleccionar pokemons");
        state = BattleState.PartySelectScreen;
        partiHUD.SetPartyData(playerParty.Pokemons);
        partiHUD.gameObject.SetActive(true);
        currentSelectedPokemon = playerParty.GetPositionFromPokemon(playerunit.Pokemon);

        partiHUD.UPdateSelectedPokemon(currentSelectedPokemon);

        if (Input.GetAxisRaw("Cancel") != 0)
        {
            PlayerActionSelection();
        }
    }

    private void OpenInventoryScreen()
    {
        print("Abrir inventario");
        batteDialogBox.ToggleActions(false);
        StartCoroutine(ThrowPokeball());
    }

    //--------------------------------------------


    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;


        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

        if (batteDialogBox.isWriting)
        {
            return;
        }

        if (state == BattleState.ActionSelection)
        {
            
            HandlePlayerActionsSelection();
        }
        else if (state == BattleState.MovementSelection)
        {
            
            HandlePlayerMovementSelector();
        }
        else if (state == BattleState.PartySelectScreen)
        {

            handlePlayerPartySelection();
        }
        else if (state == BattleState.LoseTurn)
        {
            StartCoroutine( PerformEnemyMovement());
        }
        else if (state == BattleState.ForgetMovement)
        {
            //-----Ejecutar el evento----------------
            selectMoveUI.HandleForegetMoveSelection((moveIndex) => {
                if (moveIndex < 0)
                {
                    timeSinceLastClick = 0;
                    return;
                }

                selectMoveUI.gameObject.SetActive(false);

                if (moveIndex == PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
                {
                    //No apreder el nuevo
                    StartCoroutine(batteDialogBox.SetDialog($"{playerunit.Pokemon.Base.Name} no ha aprendido {moveToLearn.Name}"));
                }
                else
                {
                    //Olvido el selecionado y aprende el nuevo
                    var seletectedMove = playerunit.Pokemon.Moves[moveIndex].Base;

                    StartCoroutine(batteDialogBox.SetDialog($"{playerunit.Pokemon.Base.Name} olvidó {seletectedMove.Name} y aprendió {moveToLearn.Name}"));
                    playerunit.Pokemon.Moves[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;
                state = BattleState.FinishBattle;
            });
            //---------------------------------------------
        }
    }

    //------ Menus de UI  - controles de menus selecionar ataque ------------
    private void HandlePlayerActionsSelection()
    {
      

        //----------- <-> ----------------------------
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;

            currentSelecteAction = (currentSelecteAction + 2) % 4;

            batteDialogBox.SelectAction(currentSelecteAction);

            //-----------------------------------------
        }

        //-------------------- || ------------------
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {

            timeSinceLastClick = 0;

            currentSelecteAction = (currentSelecteAction + 1) % 2 + 2 * Mathf.FloorToInt(currentSelecteAction / 2);
            batteDialogBox.SelectAction(currentSelecteAction);
        }
        //-----------------------------------------

        //--- aciones ------------
        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            if (currentSelecteAction == 0)
            {
                PlayerMovementSelection();//luchar
            }
            else if (currentSelecteAction == 1)
            {
                //Pokemon
                OpenPartySelectionScreen();
            }
            else if (currentSelecteAction == 2)
            {
                //mochila
                OpenInventoryScreen();
            }
            else if (currentSelecteAction == 3)
            {
                //huir
                StartCoroutine(TryToEscapeFromBattle());
            }
            //------------------------


        }

    }

    private void HandlePlayerMovementSelector()
    {
          

        //----------- <> ---------------------
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            
            timeSinceLastClick = 0;
            //var oldSelectedMovement = (curentSelectedMovement+1)%2 + 2* Mathf.FloorToInt(curentSelectedMovement/2);
            var oldSelectedMovement = currentSelectedMovement;

            currentSelectedMovement = (currentSelectedMovement + 2) % 4;
            

            if (currentSelectedMovement >= playerunit.Pokemon.Moves.Count)
            {
                
                currentSelectedMovement = oldSelectedMovement;
            }

            //PIntar de color la selecion y punto del ataue y tipo
            batteDialogBox.SelectMovement(currentSelectedMovement, playerunit.Pokemon.Moves[currentSelectedMovement]);
        }
        //---------------------------------

        //-------------------- || -------------------
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            
            timeSinceLastClick = 0;

            var oldSelectedMovement = currentSelectedMovement;

            currentSelectedMovement= (currentSelectedMovement + 1) % 2 + 2 * Mathf.FloorToInt(currentSelectedMovement / 2);

            

            if (currentSelectedMovement >= playerunit.Pokemon.Moves.Count)
            {
                
                currentSelectedMovement = oldSelectedMovement;
            }

            //PIntar de color la selecion y punto del ataue y tipo
            batteDialogBox.SelectMovement(currentSelectedMovement, playerunit.Pokemon.Moves[currentSelectedMovement]);
        }
        //--------------------------------------------

        //--------------- ejecutar movimiento ------
        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            batteDialogBox.ToggleMovements(false);
            batteDialogBox.ToggleDialogText(true);

            StartCoroutine(PerformPlayerMovement());
        }
        //---------------------------------------

        //------- cancelar ----------------------
        if (Input.GetAxisRaw("Cancel") != 0)
        {
            PlayerActionSelection();
        }
        //---------------------------------------
    }

    private void handlePlayerPartySelection()
    {

        // 0  1
        // 2  3
        // 4  5

        //----------- <> ---------------------
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPokemon -= (int)Input.GetAxisRaw("Vertical") * 2;
        }
        //---------------------------------

        //-------------------- || -------------------
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPokemon += (int)Input.GetAxisRaw("Horizontal");

        }
        //--------------------------------------------

        currentSelectedPokemon = Mathf.Clamp(currentSelectedPokemon, 0, playerParty.Pokemons.Count - 1);

        partiHUD.UPdateSelectedPokemon(currentSelectedPokemon);

        //--------------- Selecionar pokemon para batalla ------
        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            var selectedPokemon = playerParty.Pokemons[currentSelectedPokemon];

            if (selectedPokemon.Hp <= 0)
            {
                partiHUD.SetMessage("No puedes enviar un pokemon dibiliado");
                return;
            }
            else if (selectedPokemon == playerunit.Pokemon)
            {
                partiHUD.SetMessage("No puedes seleccionar el pokemon en batalla");
                return;
            }

            partiHUD.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwichPokemon(selectedPokemon));
        }
        //---------------------------------------

        //------- cancelar ----------------------
        if (Input.GetAxisRaw("Cancel") != 0)
        {
            partiHUD.gameObject.SetActive(false);
            PlayerActionSelection();
        }
        //---------------------------------------
    }
    //------------------------------------------------

    //---------------- Atake jugador y enemigo ------------
    private IEnumerator PerformPlayerMovement()
    {
        state = BattleState.PerformMovement;

        Move move = playerunit.Pokemon.Moves[currentSelectedMovement];

        if (move.Pp <=0)
        {
            PlayerMovementSelection();
            yield break; //salir de la corrutina
        }

        yield return RunMovement(playerunit, enmeyUnit, move);

        if (state == BattleState.PerformMovement)
        {
            StartCoroutine(PerformEnemyMovement());
        }

    }
    private IEnumerator PerformEnemyMovement()
    {
        state = BattleState.PerformMovement;

        Move move = enmeyUnit.Pokemon.RandomMove();

        yield return RunMovement(enmeyUnit, playerunit, move);

        if (state == BattleState.PerformMovement)
        {
            PlayerActionSelection();
        }
        
    }

    private IEnumerator RunMovement(BattleUnit attackUnit, BattleUnit target, Move move)
    {

        move.Pp--;
        yield return batteDialogBox.SetDialog($"{attackUnit.Pokemon.Base.Name} ha usado {move.Base.Name}");

        var oldHPVal = target.Pokemon.Hp;

        attackUnit.PlayAttackAnimation();

        yield return new WaitForSeconds(1f);

        target.PlayReciveAttackAnimation();

        var damageDesc = target.Pokemon.ReceiveDamage(attackUnit.Pokemon, move);

        yield return target.Hud.UpdatePokemonData(oldHPVal);

        yield return showDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return HandlePokemonFainted(target);
        }
    }
    //-----------------------------------------------------

    private void CheckForBattleFinish(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayer)
        {
            var nextPokemon = playerParty.GetFirstNonFaintedPokemon();

            if (nextPokemon != null)
            {
                OpenPartySelectionScreen();
            }
            else
            {
                BattleFinish(false);//hemos perdido la batalla
            }
        }
        else
        {
            BattleFinish(true); //hemos ganado
        }
    }

    private IEnumerator SwichPokemon(Pokemon newPokemon)
    {
        if (playerunit.Pokemon.Hp > 0)
        {
            yield return batteDialogBox.SetDialog($"Vulve {playerunit.Pokemon.Base.Name}");
            playerunit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }
        

        //-- refresaca el nuevo pokemon para la batalla
        playerunit.SetupPokemon(newPokemon);        
        batteDialogBox.SetPokemonMovements(newPokemon.Moves);


        yield return batteDialogBox.SetDialog($"¡Ve {newPokemon.Base.Name}!");

        StartCoroutine(PerformEnemyMovement());
    }

    private IEnumerator showDamageDescription(DameDescription desc)
    {
        if (desc.Critical > 1f)
        {
            yield return batteDialogBox.SetDialog("¡Un golpe critico!");
        }
        if (desc.Type > 1)
        {
            yield return batteDialogBox.SetDialog("¡Es super efectivo!");
        }
        else if (desc.Type < 1)
        {
            yield return batteDialogBox.SetDialog("No es muy efectivo...");
        }
        
    }

    //------ pokeball--------------------------------
    private IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (type != BattleType.wildPokemon)
        {
            yield return batteDialogBox.SetDialog("No puedes robar los pkemon de otros entrenadores");
            state = BattleState.LoseTurn;
            yield break;
        }

        yield return batteDialogBox.SetDialog($"¡Has lanzado una {pokeball.name}!");

        var pokeballInst = Instantiate(pokeball, playerunit.transform.position + new Vector3(-2,-2), Quaternion.identity);

        var pokeballSpt = pokeballInst.GetComponent<SpriteRenderer>();

        //Animacion---------
        yield return pokeballSpt.transform.DOLocalJump(enmeyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();

        yield return enmeyUnit.PlayCapturedAnimation();

        yield return pokeballSpt.transform.DOLocalMoveY(enmeyUnit.transform.position.y -2f, 0.3f).WaitForCompletion();

        var numberOfShakes = TryToCatchPokemon(enmeyUnit.Pokemon);
        for (int i = 0; i < Mathf.Min(numberOfShakes,3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeballSpt.transform.DOPunchRotation(new Vector3(0, 0, 15f), 0.6f).WaitForCompletion();
        }

        if (numberOfShakes == 4)
        {
            yield return batteDialogBox.SetDialog($"¡{enmeyUnit.Pokemon.Base.Name} capturado!");
            yield return pokeballSpt.DOFade(0, 1.5f).WaitForCompletion();

            if(playerParty.AddPokemonToParty(enmeyUnit.Pokemon))
            {
                yield return batteDialogBox.SetDialog($"{enmeyUnit.Pokemon.Base.Name} se ha añadido a tu equipo");
            }
            else
            {
                yield return batteDialogBox.SetDialog("En algun momvento, lo mandaremos al Pc de Bill...");
            }

            Destroy(pokeballInst);
            BattleFinish(true);
        }
        else
        {
            //El pokemon se escapa....
            yield return new WaitForSeconds(0.5f);
            pokeballSpt.DOFade(0, 0.2f);
            yield return enmeyUnit.PlayBreakOutAnimation();

            if (numberOfShakes <2)
            {
                yield return batteDialogBox.SetDialog($"{enmeyUnit.Pokemon.Base.Name} ha escapado!");
            }
            else
            {
                yield return batteDialogBox.SetDialog($"¡Casa lo has atrapado!");
            }
            Destroy(pokeballInst);

            state = BattleState.LoseTurn;
        }
    }

    private int TryToCatchPokemon(Pokemon pokemon)
    {
        float bonusPokeball = 1;
        float bonusStart = 1;
        float a = (3 * pokemon.MaxHp - 2 * pokemon.Hp) * pokemon.Base.CatchRate * bonusPokeball * bonusStart/(3*pokemon.MaxHp);

        if (a >= 255)
        {
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 4;//(0 por defecto, capturar rapido un pokemon)

        while(shakeCount < 4)
        {
            if (Random.Range(0, 65535) >=b)
            {
                break;
            }
            else
            {
                shakeCount++;
            }
        }
        return shakeCount;
    }

    private IEnumerator TryToEscapeFromBattle()
    {
        state = BattleState.Busy;

        if (type != BattleType.wildPokemon)
        {
            yield return batteDialogBox.SetDialog("No puedes huir de combates contra entrenadors Pokemon");
            state = BattleState.LoseTurn;
            yield break;
        }

        //es contra un pokemon salvage
        escapeAttemps++;

        int playerSpeed = playerunit.Pokemon.Speed;
        int enemySpeed = enmeyUnit.Pokemon.Speed;

        if (playerSpeed >= enemySpeed)
        {
            yield return batteDialogBox.SetDialog("Has escapado con exito(1)");
            yield return new WaitForSeconds(1);
            OnBattleFinish(true);
        }
        else
        {
            int olddsScape = (Mathf.FloorToInt(playerSpeed * 128 / enemySpeed) + 30 * escapeAttemps) % 256;

            if (Random.Range(0,256) < olddsScape)
            {
                yield return batteDialogBox.SetDialog("Has escapado con exito(2)");
                yield return new WaitForSeconds(1);
                OnBattleFinish(true);
            }
            else
            {
                yield return batteDialogBox.SetDialog("NO pudes escapar");
                state = BattleState.LoseTurn;
            }
        }
    }
    //--------------------------------------------------

    private IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return batteDialogBox.SetDialog($"{faintedUnit.Pokemon.Base.Name} se ha debilitado");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(1.5f);
        
        if (!faintedUnit.IsPlayer)
        {
            //Exp ++
            int expBase = faintedUnit.Pokemon.Base.ExpBase;
            int level = faintedUnit.Pokemon.Level;
            float multplier = (type == BattleType.wildPokemon ? 1 : 1.5f);

            int wonExp = Mathf.FloorToInt(expBase * level * multplier / 7);//expericia ganada del enemigo

            playerunit.Pokemon.Experience += wonExp;//suma la experiencia ganada
            yield return batteDialogBox.SetDialog($"{playerunit.Pokemon.Base.Name} ha ganado {wonExp} punto de experiencia");

            yield return playerunit.Hud.SetExpSmooth();//subier la experiencia ganada en la ui

            yield return new WaitForSeconds(0.5f);

            //chequear New level
            while (playerunit.Pokemon.NeedsToLevelUp())
            {
                playerunit.Hud.SetLevelText();
                yield return playerunit.Hud.UpdatePokemonData(playerunit.Pokemon.Hp);
                yield return batteDialogBox.SetDialog($"{playerunit.Pokemon.Base.Name} sube de nivel");

                //Intentar aprende un nuevo movimiento
                var newLearnalbeMove = playerunit.Pokemon.GetLearnablemOveAtCurentLevel();
                if (newLearnalbeMove != null)
                {
                    if (playerunit.Pokemon.Moves.Count < PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
                    {
                        //podemos aprender el movimiento
                        playerunit.Pokemon.learMove(newLearnalbeMove);
                        yield return batteDialogBox.SetDialog($"{playerunit.Pokemon.Base.Name} ha aprendido {newLearnalbeMove.Move.Name}");
                        batteDialogBox.SetPokemonMovements(playerunit.Pokemon.Moves);
                    }
                    else
                    {
                        //olvidar uno de los movimientos
                        yield return batteDialogBox.SetDialog($"{playerunit.Pokemon.Base.Name} intenta aprender {newLearnalbeMove.Move.Name}");
                        yield return batteDialogBox.SetDialog($"Pero no puede aprender mas de {PokemonBase.NUMBER_OF_LEARNABLE_MOVES} movimientos");
                        yield return ChooseMovementToForget(playerunit.Pokemon, newLearnalbeMove.Move);
                        yield return new WaitUntil(() => state != BattleState.ForgetMovement);//delegado
                    }
                }

                yield return playerunit.Hud.SetExpSmooth(true);
            }
        }

        CheckForBattleFinish(faintedUnit);
    }

    private IEnumerator ChooseMovementToForget(Pokemon learner, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return batteDialogBox.SetDialog("Seleciona del movimiento que quieres olvidar");
        selectMoveUI.gameObject.SetActive(true);
        selectMoveUI.SetMovements(learner.Moves.Select(mv => mv.Base).ToList(),newMove);
        moveToLearn = newMove;
        state = BattleState.ForgetMovement;
    }

    /*
    private IEnumerator ForgetOldMove()
    {

    }
    */
}
