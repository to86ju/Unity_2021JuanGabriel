using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager ShareInstance;

    [SerializeField]
    [Tooltip("Cantidad de putnos de la partida actual")]
    private int amount;

    public int Amount { get => amount; set => amount = value; }

    private void Awake()
    {
        if(ShareInstance == null)
        {
            ShareInstance = this;
        }
        else
        {
            Debug.LogWarning("ScoreManager duplicado; debe ser destruido", gameObject);
            Destroy(gameObject);
        }

    }
}
