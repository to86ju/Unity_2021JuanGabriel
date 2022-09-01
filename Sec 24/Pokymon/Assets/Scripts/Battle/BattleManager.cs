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
    [SerializeField]private BattleUnit playerunit;
    [SerializeField]private BattleHUD playerHUD;

    [SerializeField]private BattleUnit enmeyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox batteDialogBox;

    public BattleState state;

    private void Start()
    {
        StartCoroutine( SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.StarBattle;

        playerunit.SetupPokemon();
        playerHUD.SetPokemonData(playerunit.Pokemon);

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
        batteDialogBox.ToggleActions(true);
    }

    private void EnemyAction()
    {

    }
}
