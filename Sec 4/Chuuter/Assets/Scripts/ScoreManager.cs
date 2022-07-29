using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager ShareInstance;//singleton

    [SerializeField]
    [Tooltip("Cantidad de puntos de la partida actual")]
    private int amount;

    public int Amount { get => amount; set => amount = value; }

    private void Awake()
    {
        //si el ShareInstance es nulo
        if (ShareInstance == null)
        {
            ShareInstance = this;//crear el ShareInstance
        }
        else
        {
            Debug.LogWarning("ScoreManager duplicado; debe ser destruido", gameObject);
            Destroy(gameObject);
        }

    }
}
