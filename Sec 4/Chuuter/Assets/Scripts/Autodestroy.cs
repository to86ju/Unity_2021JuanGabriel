using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autodestroy : MonoBehaviour
{
    [Tooltip("Tiempo después del cual se destruye el objeto")]
    public float delay;


    void OnEnable()
    {
        //Destroy(gameObject,delay);
        Invoke("HideObjects", delay);
    }

    public void HideObjects()
    {
        gameObject.SetActive(false);
    }
}
