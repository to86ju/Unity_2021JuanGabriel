using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public GameObject healthBar;

    public Color BarColor
    {
        get
        {
            var localscale = healthBar.transform.localScale.x;
            if (localscale < 0.15f)
            {
                return new Color(193f / 255, 45f / 255, 45f / 255);
            }
            else if (localscale < 0.5f)
            {
                return new Color(211f / 255, 211f / 255, 29f / 255);
            }
            else
            {
                return new Color(98f/255, 178f/255,61f/255);
            }
        }
    }

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
            healthBar.GetComponent<Image>().color = BarColor;
            yield return null;

        }
        healthBar.transform.localScale = new Vector3(normalizeValue, 1);
    }
}
