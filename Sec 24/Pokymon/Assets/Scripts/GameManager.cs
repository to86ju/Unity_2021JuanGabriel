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
    private GameState _gameState;

    private void Awake()
    {
        _gameState = GameState.Travel;
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
