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

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.StarBattle;

        playerunit.SetupPokemon();
        playerHUD.SetPokemonData(playerunit.Pokemon);

        batteDialogBox.SetPokemonMovements(playerunit.Pokemon.Move);

        enmeyUnit.SetupPokemon();
        enemyHUD.SetPokemonData(enmeyUnit.Pokemon);

        yield return batteDialogBox.SetDialog($"Un {enmeyUnit.Pokemon.Base.name} salvaje apareció.");

        yield return new WaitForSeconds(1.0f);

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
    }

    private void EnemyAction()
    {

    }

    private void Update()
    {
        timeSinceLastClick += Time.deltaTime;

        if (state == BattleState.PlayerSelctAction)
        {
            HandlePlayerActionsSelection();
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
                Debug.Log("luchar");
                PlayerMovement();
            }
            else if (currentSelecteAction == 1)
            {
                //Implementar la huida
            }
        }

    }
}
