using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField]private BattleUnit playerunit;
    [SerializeField]private BattleHUD playerHUD;

    [SerializeField]private BattleUnit enmeyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox batteDialogBox;

    private void Start()
    {
        SetupBattle();
    }

    public void SetupBattle()
    {
        playerunit.SetupPokemon();
        playerHUD.SetPokemonData(playerunit.Pokemon);

        enmeyUnit.SetupPokemon();
        enemyHUD.SetPokemonData(enmeyUnit.Pokemon);

        StartCoroutine( batteDialogBox.SetDialog($"Un {enmeyUnit.Pokemon.Base.name} salvaje apareció."));
    }

}
