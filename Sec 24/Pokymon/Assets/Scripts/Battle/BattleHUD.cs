using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text pokemonName;
    public Text pokemonLevel;
    public HealthBar healthbar;    
    private Pokemon _pokemon;
    public GameObject statusBox;
    public GameObject expBar;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        pokemonName.text = pokemon.Base.Name;
        //pokemonLevel.text = $"Lv {pokemon.Level}";
        SetLevelText();
        healthbar.SetHP(_pokemon);        
        SetExp();//escalar la barra de experiencia nada mas cargar        
        StartCoroutine( UpdatePokemonData());
        SetStatusConditionData();

        _pokemon.OnStatusConditionChanged += SetStatusConditionData;
    }

    public IEnumerator UpdatePokemonData()
    {
        if (_pokemon.HasHPChanged)
        {
            yield return healthbar.SetSmoothHP(_pokemon);            
            
            _pokemon.HasHPChanged = false;
        }
        
    }

    public void SetExp()
    {
        if (expBar == null)
        {
            return;
        }

        //Escalado de la barra
        expBar.transform.localScale = new Vector3(NormalizeExp(), 1, 1);
    }

    public IEnumerator SetExpSmooth(bool needsToResetBar = false)
    {
        if (expBar == null)
        {
            yield break;
        }

        if (needsToResetBar)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }


        //escalado de la barra animada
        yield return expBar.transform.DOScaleX(NormalizeExp(),2f).WaitForCompletion();
    }

    //Metodo para scalar la varra correctamente
    private float NormalizeExp()
    {
        //experiencia actual(min)
        float currentLevelExp = _pokemon.Base.GetNecessaryExpForLevel(_pokemon.Level);        

        //experiencia necesaria para llegar al siguiente nivel(max)
        float nextLevelExp = _pokemon.Base.GetNecessaryExpForLevel(_pokemon.Level + 1);       

        //experiencia normalizada
        float normalizedExp = (_pokemon.Experience - currentLevelExp) / (nextLevelExp - currentLevelExp);

        
        //Recorta el valor para que lo mas pequeño sea 0 y viceversa sea 1
        return Mathf.Clamp01(normalizedExp);
    }

    public void SetLevelText()
    {
        pokemonLevel.text = $"Lv {_pokemon.Level}";
    }

    private void SetStatusConditionData()
    {
        if (_pokemon.statusCodition == null)
        {
            statusBox.SetActive(false);
        }
        else
        {
            statusBox.SetActive(true);
            statusBox.GetComponent<Image>().color = ColorManager.StatusConditionColor.GetColorFromStatusCondition(_pokemon.statusCodition.Id);
            statusBox.GetComponentInChildren<Text>().text = _pokemon.statusCodition.Id.ToString().ToUpper();
        }
    }
}
