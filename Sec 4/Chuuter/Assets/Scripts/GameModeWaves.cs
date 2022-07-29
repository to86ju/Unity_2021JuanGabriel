using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeWaves : MonoBehaviour
{
    [SerializeField]
    private life playerLife;

    [SerializeField]
    private life baseLife;

    private void Awake()
    {
        playerLife.onDeath.AddListener(CheckLoseCondition);//listener o vinculacion evento muerte del player
        baseLife.onDeath.AddListener(CheckLoseCondition);//listener o vinculacion evento muerte de la base

    }

    private void Start()
    {
        EnemyManager.SharedInstance.onEnemyChyanged.AddListener(CheckWinCondition);//listener o vinculacion para cambiar la lista enemigo
        WaveManager.ShareInstance.onWaveChanged.AddListener(CheckWinCondition);//listener o vinculacion para cambiar la lista oleadas
    }

    //funcion para saber si el juegador a muerto
    void CheckLoseCondition()
    {
        RegisterScore();
        SceneManager.LoadScene("LoseScene", LoadSceneMode.Single);
    }

    //funcion para saber si el jugador a ganado el juego
    void CheckWinCondition()
    {

        //Ganar
        if (EnemyManager.SharedInstance.Enemycount <= 0 && WaveManager.ShareInstance.WavesCount <= 0)
        {
            Debug.Log("gane");
            RegisterScore();
            SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
        }
    }

    void RegisterScore()
    {
        var acutualScore = ScoreManager.ShareInstance.Amount;
        PlayerPrefs.SetInt("Last Score", acutualScore);

        var highScore = PlayerPrefs.GetInt("High Score", 0);

        if (acutualScore > highScore)
        {
            PlayerPrefs.SetInt("High Score", acutualScore);
        }
    }

    void RegisterTime()
    {
        var acutualTime = Time.time;
        PlayerPrefs.SetFloat("Last Time", acutualTime);

        var LowTime = PlayerPrefs.GetFloat("Low Time", 999999999.0f);

        if (acutualTime < LowTime)
        {
            PlayerPrefs.SetFloat("Low Time", LowTime);
        }
    }
}
