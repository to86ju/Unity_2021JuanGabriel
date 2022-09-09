using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    StarBattle,
    PlayerSelctAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerunit;
    [SerializeField] private BattleHUD playerHUD;

    [SerializeField] private BattleUnit enmeyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox batteDialogBox;

    public BattleState state;

    private int currentSelecteAction;
    private float timeSinceLastClick;
    public float timeBetweenClicks = 1.0f;

    private int curentSelectedMovement;

    public event Action<bool> OnBattleFinish;

    public void HandleStartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.StarBattle;

        //------------- player ---------------
        playerunit.SetupPokemon(playerunit.Pokemon);
        playerHUD.SetPokemonData(playerunit.Pokemon);

        batteDialogBox.SetPokemonMovements(playerunit.Pokemon.Move);
        //----------------------------

        //----------- enemy -----------------
        enmeyUnit.SetupPokemon(enmeyUnit.Pokemon);
        enemyHUD.SetPokemonData(enmeyUnit.Pokemon);
        //------------------------------------------

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
        state = BattleState.PlayerMove;
        batteDialogBox.ToggleDialogText(false);
        batteDialogBox.ToggleActions(false);
        batteDialogBox.ToggleMovements(true);
        curentSelectedMovement = 0;
        batteDialogBox.SelectMovement(curentSelectedMovement,playerunit.Pokemon.Move[curentSelectedMovement]);
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
            OnBattleFinish(false);
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
            HandlePlayerActionsSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandlePlayerMovementSelector();
        }
    }

    private void HandlePlayerActionsSelection()
    {
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;

            currentSelecteAction = (currentSelecteAction+1) % 2;

            batteDialogBox.SelectAction(currentSelecteAction);

        }

        if (Input.GetAxisRaw("Submit") !=0)
        {
            timeSinceLastClick = 0;
            if (currentSelecteAction == 0)
            {
                PlayerMovement();
            }
            else if (currentSelecteAction == 1)
            {
                //Implementar la huida
            }
        }


    }
    private void HandlePlayerMovementSelector()
    {
        if(timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical")!=0)
        {
            timeSinceLastClick = 0;
            var oldSelectedMovement = curentSelectedMovement;

            curentSelectedMovement = (curentSelectedMovement + 2) % 4;

            if (curentSelectedMovement >= playerunit.Pokemon.Move.Count)
            {
                curentSelectedMovement = oldSelectedMovement;
            }

            batteDialogBox.SelectMovement(curentSelectedMovement,playerunit.Pokemon.Move[curentSelectedMovement]);
        }

        else if (Input.GetAxisRaw("Horizontal")!= 0)
        {
            timeSinceLastClick = 0;

            var oldSelectedMovement = curentSelectedMovement;

            if (curentSelectedMovement <=1)
            {
                curentSelectedMovement = (curentSelectedMovement + 1) % 2;
            }
            else //curentSelectedMovement >= 0
            {
                curentSelectedMovement = (curentSelectedMovement - 1) % 2 + 2;
            }

            if (curentSelectedMovement >= playerunit.Pokemon.Move.Count)
            {
                curentSelectedMovement = oldSelectedMovement;
            }

            batteDialogBox.SelectMovement(curentSelectedMovement, playerunit.Pokemon.Move[curentSelectedMovement]);
        }

        //--------------- ejecutar movimiento ------
        if (Input.GetAxisRaw("Submit") !=0)
        {
            timeSinceLastClick = 0;
            batteDialogBox.ToggleMovements(false);
            batteDialogBox.ToggleDialogText(true);

            StartCoroutine(PerformPlayerMovement());
        }
        //---------------------------------------
    }

    private IEnumerator PerformPlayerMovement()
    {
        Move move = playerunit.Pokemon.Move[curentSelectedMovement];
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
