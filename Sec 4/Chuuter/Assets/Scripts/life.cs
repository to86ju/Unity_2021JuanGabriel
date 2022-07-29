using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class life : MonoBehaviour
{
    [SerializeField]
    private float amount;//puntos de vida

    public float maximumLife = 100f;//vida maxima
    public UnityEvent onDeath;//Evento muerte
    
    public float Amount
    {
        get => amount;
        set {
            amount = value;

            if(amount <= 0)
            {
                onDeath.Invoke();//Lanzar evento de muerte 
            }
        }
      
    }

    private void Awake()
    {
        amount = maximumLife;//cantidad inicial de vida del jugador
    }

}
