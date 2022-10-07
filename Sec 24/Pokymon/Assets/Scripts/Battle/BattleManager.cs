using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    StarBattle,
    PlayerSelctAction,
    PlayerSelectMove,
    EnemyMove,
    Busy,
    PartySelectScreen
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

    private int currentSelecteAction;
    private float timeSinceLastClick;
    public float timeBetweenClicks = 1.0f;

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

        PlayerAction();
    }

    private void PlayerAction()
    {
        state = BattleState.PlayerSelctAction;
        StartCoroutine(batteDialogBox.SetDialog("Seleciona una action"));
        batteDialogBox.ToggleDialogText(true);
        batteDialogBox.ToggleActions(true);
        batteDialogBox.ToggleMovements(false);
        currentSelecteAction = 0;
        batteDialogBox.SelectAction(currentSelecteAction);
    }

    private void PlayerMovement()
    {
        state = BattleState.PlayerSelectMove;
        batteDialogBox.ToggleDialogText(false);
        batteDialogBox.ToggleActions(false);
        batteDialogBox.ToggleMovements(true);
        currentSelectedMovement = 0;
        batteDialogBox.SelectMovement(currentSelectedMovement,playerunit.Pokemon.Move[currentSelectedMovement]);
    }

    private IEnumerator EnemyAction()
    {
        state = BattleState.EnemyMove;

        Move move = enmeyUnit.Pokemon.RandomMove();
        move.Pp--;

        yield return batteDialogBox.SetDialog($"{enmeyUnit.Pokemon.Base.Name} ha usado {move.Base.Name}");

        var oldHPVal = playerunit.Pokemon.Hp;

        enmeyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        playerunit.PlayReciveAttackAnimation();

        var damageDesc = playerunit.Pokemon.ReceiveDamage(enmeyUnit.Pokemon, move);

        playerHUD.UpdatePokemonData(oldHPVal);

        yield return showDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return batteDialogBox.SetDialog($"{playerunit.Pokemon.Base.Name} ha sido debilitado.");
            playerunit.PlayFaintAnimation();

            yield return new WaitForSeconds(1.5f);

            var nextPokemon = playerParty.GetFirstNonFaintedPokemon();//siguiete pokemon con vida

            if (nextPokemon == null)
            {
                OnBattleFinish(false);

            }
            else
            {
                playerunit.SetupPokemon(nextPokemon);
                playerHUD.SetPokemonData(nextPokemon);

                batteDialogBox.SetPokemonMovements(nextPokemon.Move);

                yield return batteDialogBox.SetDialog($"¡Alelante {nextPokemon.Base.Name}!");

                PlayerAction();
            }

        }
        else
        {
            PlayerAction();
        }
    }

    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;

        if (batteDialogBox.isWriting)
        {
            return;
        }

        if (state == BattleState.PlayerSelctAction)
        {
            Debug.Log("menu de selecion");
            HandlePlayerActionsSelection();
        }
        else if (state == BattleState.PlayerSelectMove)
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
            currentSelectedPokemon -= (int)Input.GetAxisRaw("Vertical")*2;            
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
            PlayerAction();
        }
        //---------------------------------------
    }

    private IEnumerator SwichPokemon(Pokemon newPokemon)
    {
        yield return batteDialogBox.SetDialog($"Vulve {playerunit.Pokemon.Base.name}");
        playerunit.PlayFaintAnimation();
        yield return new WaitForSeconds(1.5f);

        //-- refresaca el nuevo pokemon para la batalla
        playerunit.SetupPokemon(newPokemon);
        playerHUD.SetPokemonData(newPokemon);
        batteDialogBox.SetPokemonMovements(newPokemon.Move);


        yield return batteDialogBox.SetDialog($"¡Ve {newPokemon.Base.name}!");

        StartCoroutine(EnemyAction());
    }

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

            currentSelecteAction = (currentSelecteAction+2) % 4;            

            batteDialogBox.SelectAction(currentSelecteAction);

        //-----------------------------------------
        }

        //-------------------- || ------------------
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            
            timeSinceLastClick = 0;

            currentSelecteAction = (currentSelecteAction + 1)%2 + 2 * Mathf.FloorToInt(currentSelecteAction / 2);
            batteDialogBox.SelectAction(currentSelecteAction);            
        }
        //-----------------------------------------

        //--- aciones ------------
        if (Input.GetAxisRaw("Submit") !=0)
        {
            timeSinceLastClick = 0;
            if (currentSelecteAction == 0)
            {
                PlayerMovement();//luchar
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

    private void OpenInventoryScreen()
    {
        print("Abrir inventario");

        if (Input.GetAxisRaw("Cancel") !=0)
        {
            PlayerAction();
        }
    }

    private void OpenPartySelectionScreen()
    {
        //print("Abrir la pantalla para seleccionar pokemons");
        state = BattleState.PartySelectScreen;
        partiHUD.SetPartyData(playerParty.Pokemons);        
        partiHUD.gameObject.SetActive(true);
        currentSelectedPokemon = 0;

        // ------ obtener la posicion del pokemon actual -----
        for (int i = 0; i < playerParty.Pokemons.Count; i++)
        {
            if (playerunit.Pokemon == playerParty.Pokemons[i])
            {
                currentSelectedPokemon = i;
            }
        }
        //-------------------------------------------------------

        partiHUD.UPdateSelectedPokemon(currentSelectedPokemon);

        if (Input.GetAxisRaw("Cancel") != 0)
        {
            PlayerAction();
        }
    }

    private void HandlePlayerMovementSelector()
    {
        if(timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

        Debug.Log("currentSelectedMovement " + currentSelectedMovement);

        //----------- <> ---------------------
        if (Input.GetAxisRaw("Vertical")!=0)
        {
            Debug.Log("Vertical axis HPMS " + Input.GetAxisRaw("Vertical"));
            timeSinceLastClick = 0;
            //var oldSelectedMovement = (curentSelectedMovement+1)%2 + 2* Mathf.FloorToInt(curentSelectedMovement/2);
            var oldSelectedMovement = currentSelectedMovement;


            currentSelectedMovement = (currentSelectedMovement + 2) % 4;
            Debug.Log("HPMS v " + currentSelectedMovement);

            if (currentSelectedMovement >= playerunit.Pokemon.Move.Count)
            {
                Debug.Log("el pokemon tiene pocos moviminetos aprendidos "+ playerunit.Pokemon.Move.Count);
                currentSelectedMovement = oldSelectedMovement;
            }

            //PIntar de color la selecion y punto del ataue y tipo
            batteDialogBox.SelectMovement(currentSelectedMovement,playerunit.Pokemon.Move[currentSelectedMovement]);
        }
        //---------------------------------

        //-------------------- || -------------------
        else if (Input.GetAxisRaw("Horizontal")!= 0)
        {
            Debug.Log("Horizontal axis HPMS " + Input.GetAxisRaw("Horizontal"));
            timeSinceLastClick = 0;

            var oldSelectedMovement = (currentSelectedMovement+1)%2 + 2*Mathf.FloorToInt(currentSelectedMovement/2);

            Debug.Log("HPMS H " + " old " + oldSelectedMovement + " cu "+ currentSelectedMovement);

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
        if (Input.GetAxisRaw("Submit") !=0)
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
            PlayerAction();
        }
        //---------------------------------------
    }

    private IEnumerator PerformPlayerMovement()
    {
        Move move = playerunit.Pokemon.Move[currentSelectedMovement];
        move.Pp--;
        yield return batteDialogBox.SetDialog($"{playerunit.Pokemon.Base.Name} ha usado {move.Base.Name}");

        var oldHPVal = enmeyUnit.Pokemon.Hp;

        playerunit.PlayAttackAnimation();

        yield return new WaitForSeconds(1f);

        enmeyUnit.PlayReciveAttackAnimation();

        var damageDesc = enmeyUnit.Pokemon.ReceiveDamage(playerunit.Pokemon, move);

        enemyHUD.UpdatePokemonData(oldHPVal);

        yield return showDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return batteDialogBox.SetDialog($"{enmeyUnit.Pokemon.Base.Name} se ha debilitado");
            enmeyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
            Debug.Log("fin de la batalla");
            OnBattleFinish(true);
        }
        else
        {
            StartCoroutine(EnemyAction());
        }
    }

    IEnumerator showDamageDescription(DameDescription desc)
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
