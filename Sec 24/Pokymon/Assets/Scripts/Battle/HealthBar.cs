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

    public IEnumerator SetSmoothHP(float normalizeValue)
    {
        float currentScale = healthBar.transform.localScale.x;
        float updateQuantity = currentScale - normalizeValue;

        while (currentScale - normalizeValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            healthBar.transform.localScale = new Vector3(currentScale, 1);
            yield return null;

        }
        healthBar.transform.localScale = new Vector3(normalizeValue, 1);
    }
}
