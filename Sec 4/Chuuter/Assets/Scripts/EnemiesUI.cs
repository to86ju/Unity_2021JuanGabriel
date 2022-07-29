using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemiesUI : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        EnemyManager.SharedInstance.onEnemyChyanged.AddListener(RefrestText);
    }

    private void RefrestText()
    {
        _text.text = "Remaing Enemies: " + EnemyManager.SharedInstance.Enemycount;
    }
}
