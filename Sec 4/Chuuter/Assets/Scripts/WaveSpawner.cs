using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Tooltip("Prefab de Enemigo a genear")]
    public GameObject prefab;

    [Tooltip("Timepo en el que se inicia y finaliza la oleada")]
    public float startTime, endTime;

    [Tooltip("Tiempo entre la generación de emigos")]
    public float spawnRate;

    // Start is called before the first frame update
    void Start()
    {
        WaveManager.ShareInstance.AddWave(this);
        InvokeRepeating("SpawnEnemy", startTime, spawnRate);
        Invoke("EndWave",endTime);
    }

    //funcion para espaunear enemigos en la poscion indicada
    void SpawnEnemy()
    {
        //Movimiento aletorio(giro)
        //Quaternion q = Quaternion.Euler(0, transform.rotation.eulerAngles.y + Random.Range(-45.0f,45.0f), 0);
        
        Instantiate(prefab, transform.position, transform.rotation);
    }

    //funcion para para parar de espaunear enemigos
    void EndWave()
    {
        WaveManager.ShareInstance.RemoveWave(this);
        CancelInvoke();//cancela el invokeRepating

    }
}
