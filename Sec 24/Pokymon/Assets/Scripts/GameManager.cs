using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Travel,Battle
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] Camera worldMainCamera;
    private GameState _gameState;

    private void Awake()
    {
        _gameState = GameState.Travel;
    }

    private void Start()
    {
        //Subcriberse al evento
        playerController.OnPokemonEncountered += StartPokemonBattle;
        battleManager.OnBattleFinish += FinishPokemonBattle;
    }

    private void FinishPokemonBattle(bool playerHasWon)
    {
        
        _gameState = GameState.Travel;
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);

        if (!playerHasWon)
        {

        }
    }

    private void StartPokemonBattle()
    {
        _gameState = GameState.Battle;
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<PokemonMapArea>().GetComponent<PokemonMapArea>().GetRandomWildPokemon();

        battleManager.HandleStartBattle(playerParty,wildPokemon);
    }

    private void Update()
    {
        if (_gameState == GameState.Travel)
        {
           
            playerController.HandleUpdate();
        }
        else if (_gameState == GameState.Battle)
        {
            
            battleManager.HandleUpdate();
        }
    }
}
