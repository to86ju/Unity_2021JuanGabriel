using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    StarBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    Busy,
    PartySelectScreen,
    ItemSelectScreen,
    FinishBattle
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerunit;
    [SerializeField] private BattleHUD playerHUD;

    [SerializeField] private BattleUnit enmeyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox batteDialogBox;

    [SerializeField] private PartyHUD partiHUD;

    public BattleState state;

    
    private float timeSinceLastClick;
    [SerializeField]private float timeBetweenClicks = 1.0f;

    private int currentSelecteAction;
    private int currentSelectedMovement;
    private int currentSelectedPokemon;

    public event Action<bool> OnBattleFinish;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;

    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;

        StartCoroutine(SetupBattle());
    }

    //---------- inicia y fin de la batalla ------------
    public IEnumerator SetupBattle()
    {
        
        state = BattleState.StarBattle;

        //------------- player ---------------
        playerunit.SetupPokemon(playerParty.GetFirstNonFaintedPokemon());
        playerHUD.SetPokemonData(playerunit.Pokemon);

        batteDialogBox.SetPokemonMovements(playerunit.Pokemon.Move);
        //----------------------------

        //----------- enemy -----------------
        enmeyUnit.SetupPokemon(wildPokemon);
        enemyHUD.SetPokemonData(enmeyUnit.Pokemon);
        //------------------------------------------

        partiHUD.InitPartyHUD();//inicializar la parti de batalla

        yield return batteDialogBox.SetDialog($"Un {enmeyUnit.Pokemon.Base.name} salvaje apareció.");

        if (enmeyUnit.Pokemon.Speed > playerunit.Pokemon.Speed)
        {
            StartCoroutine(batteDialogBox.SetDialog("El enemigo ataca primero"));
            StartCoroutine(PerformEnemyMovement());
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

    //------ Menus de UI - Actiones ------------
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
        batteDialogBox.SelectMovement(currentSelectedMovement,playerunit.Pokemon.Move[currentSelectedMovement]);
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

    }

    //--------------------------------------------


    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;

        if (batteDialogBox.isWriting)
        {
            return;
        }

        if (state == BattleState.ActionSelection)
        {
            Debug.Log("menu de selecion");
            HandlePlayerActionsSelection();
        }
        else if (state == BattleState.MovementSelection)
        {
            Debug.Log("menu de batalla");
            HandlePlayerMovementSelector();
        }
        else if (state == BattleState.PartySelectScreen)
        {
            Debug.Log("menu de party");
            handlePlayerPartySelection();
        }
    }

    //------ Menus de UI - controles de menus ------------
    private void HandlePlayerActionsSelection()
    {
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

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
                OnBattleFinish(false);
            }
            //------------------------


        }

    }

    private void HandlePlayerMovementSelector()
    {
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

        Debug.Log("currentSelectedMovement " + currentSelectedMovement);

        //----------- <> ---------------------
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            Debug.Log("Vertical axis HPMS " + Input.GetAxisRaw("Vertical"));
            timeSinceLastClick = 0;
            //var oldSelectedMovement = (curentSelectedMovement+1)%2 + 2* Mathf.FloorToInt(curentSelectedMovement/2);
            var oldSelectedMovement = currentSelectedMovement;

            currentSelectedMovement = (currentSelectedMovement + 2) % 4;
            Debug.Log("HPMS v " + currentSelectedMovement);

            if (currentSelectedMovement >= playerunit.Pokemon.Move.Count)
            {
                Debug.Log("el pokemon tiene pocos moviminetos aprendidos " + playerunit.Pokemon.Move.Count);
                currentSelectedMovement = oldSelectedMovement;
            }

            //PIntar de color la selecion y punto del ataue y tipo
            batteDialogBox.SelectMovement(currentSelectedMovement, playerunit.Pokemon.Move[currentSelectedMovement]);
        }
        //---------------------------------

        //-------------------- || -------------------
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            Debug.Log("Horizontal axis HPMS " + Input.GetAxisRaw("Horizontal"));
            timeSinceLastClick = 0;

            var oldSelectedMovement = (currentSelectedMovement + 1) % 2 + 2 * Mathf.FloorToInt(currentSelectedMovement / 2);

            Debug.Log("HPMS H " + " old " + oldSelectedMovement + " cu " + currentSelectedMovement);

            if (currentSelectedMovement >= playerunit.Pokemon.Move.Count)
            {
                Debug.Log("el pokemon tiene pocos moviminetos aprendidos " + playerunit.Pokemon.Move.Count);
                currentSelectedMovement = oldSelectedMovement;
            }

            //PIntar de color la selecion y punto del ataue y tipo
            batteDialogBox.SelectMovement(currentSelectedMovement, playerunit.Pokemon.Move[currentSelectedMovement]);
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
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

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

    private IEnumerator PerformPlayerMovement()
    {
        state = BattleState.PerformMovement;

        Move move = playerunit.Pokemon.Move[currentSelectedMovement];

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

        //enemyHUD.UpdatePokemonData(oldHPVal); // Arreglar

        yield return showDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return batteDialogBox.SetDialog($"{target.Pokemon.Base.Name} se ha debilitado");
            target.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
            Debug.Log("fin de la batalla");
            CheckForBattleFinish(target);
        }
    }

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
            yield return batteDialogBox.SetDialog($"Vulve {playerunit.Pokemon.Base.name}");
            playerunit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }
        

        //-- refresaca el nuevo pokemon para la batalla
        playerunit.SetupPokemon(newPokemon);
        playerHUD.SetPokemonData(newPokemon);
        batteDialogBox.SetPokemonMovements(newPokemon.Move);


        yield return batteDialogBox.SetDialog($"¡Ve {newPokemon.Base.name}!");

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
}
