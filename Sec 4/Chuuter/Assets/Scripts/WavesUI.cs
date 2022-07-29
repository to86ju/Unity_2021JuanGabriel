using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WavesUI : MonoBehaviour
{

    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        WaveManager.ShareInstance.onWaveChanged.AddListener(RefrestText);
    }

    private void RefrestText()
    {
        _text.text = "Wave: " + (WaveManager.ShareInstance.MaxWaves - WaveManager.ShareInstance.WavesCount )+ "/" + WaveManager.ShareInstance.MaxWaves;
    }
}
