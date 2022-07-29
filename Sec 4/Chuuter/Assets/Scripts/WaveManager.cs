using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static WaveManager ShareInstance;//singleton
    private List<WaveSpawner> waves;

    public UnityEvent onWaveChanged;//EVENTO

    private int maxWaves;

    public int WavesCount
    {
        get => waves.Count;
    }

    public int MaxWaves
    {
        get => maxWaves;
    }

    private void Awake()
    {
        //si el ShareInstance es nula
        if (ShareInstance == null)
        {
            ShareInstance = this;//crear ShareInstance
            waves = new List<WaveSpawner>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddWave(WaveSpawner waveAdd)
    {
        maxWaves++;
        waves.Add(waveAdd);
        onWaveChanged.Invoke();//lanzar evento
    }

    public void RemoveWave(WaveSpawner waveremo)
    {
        waves.Remove(waveremo);
        onWaveChanged.Invoke();//Lanzar evento
    }
}
