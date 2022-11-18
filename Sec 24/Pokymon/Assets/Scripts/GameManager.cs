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

    public AudioClip worldclip, battleclip;

    private void Awake()
    {
        _gameState = GameState.Travel;
        
    }
    private void Start()
    {
        //Subcriberse al evento
        playerController.OnPokemonEncountered += StartPokemonBattle;
        battleManager.OnBattleFinish += FinishPokemonBattle;

        SoundManager.sharedInstance.PlayMusic(worldclip);// Musica de poblado
    }

    private void FinishPokemonBattle(bool playerHasWon)
    {
        SoundManager.sharedInstance.PlayMusic(worldclip);

        _gameState = GameState.Travel;
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);

        if (!playerHasWon)
        {

        }
    }

    private void StartPokemonBattle()
    {
        SoundManager.sharedInstance.PlayMusic(battleclip);// musica de batalla

        _gameState = GameState.Battle;
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<PokemonMapArea>().GetComponent<PokemonMapArea>().GetRandomWildPokemon();

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);//copia de pokemon

        battleManager.HandleStartBattle(playerParty,wildPokemonCopy);
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
