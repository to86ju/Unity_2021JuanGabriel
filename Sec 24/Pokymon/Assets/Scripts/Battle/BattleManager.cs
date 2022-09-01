using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField]private BattleUnit playerunit;
    [SerializeField]private BattleHUD playerHUD;

    private void Start()
    {
        SetupBattle();
    }

    public void SetupBattle()
    {
        playerunit.SetupPokemon();
        playerHUD.SetPokemonData(playerunit.Pokemon);
    }
}
