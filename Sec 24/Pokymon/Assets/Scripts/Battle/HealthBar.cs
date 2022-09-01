using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBar;

    /// <summary>
    /// Actualiza la barra de vida a partir del valor pnormalizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de lavida normalizado entre 0 y 1</param>
    public void SetHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1.0f);
    }
}
