using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public GameObject healthBar;
    public Text currentHPText;
    public Text maxHPText;



    /// <summary>
    /// Actualiza la barra de vida a partir del valor pnormalizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de lavida normalizado entre 0 y 1</param>
    public void SetHP(Pokemon pokemon)
    {
        float normalizedValue = (float)pokemon.Hp / pokemon.MaxHp;
        healthBar.transform.localScale = new Vector3(normalizedValue, 1.0f);
        healthBar.GetComponent<Image>().color = ColorManager.sharedInstance.BarColor(normalizedValue);
        currentHPText.text = pokemon.Hp.ToString();
        maxHPText.text = $"/{pokemon.MaxHp}";
    }

    public IEnumerator SetSmoothHP(Pokemon pokemon)
    {
        /*
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
        */        
        float normalizedValue = (float)pokemon.Hp / pokemon.MaxHp;
        var seq = DOTween.Sequence();//crea una sequencia
        seq.Append( healthBar.transform.DOScaleX(normalizedValue, 1f));//añado seq
        seq.Join(healthBar.GetComponent<Image>().DOColor(ColorManager.sharedInstance.BarColor(normalizedValue),1f));//Junto seq
        seq.Join(currentHPText.DOCounter(pokemon.previousHPValue,pokemon.Hp,1f));
        yield return seq.WaitForCompletion();

    }

  
}
