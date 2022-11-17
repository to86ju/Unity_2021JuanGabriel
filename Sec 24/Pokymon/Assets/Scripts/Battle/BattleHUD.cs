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
    public Text pokemonHealth;
    private Pokemon _pokemon;
    public GameObject expBar;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        pokemonName.text = pokemon.Base.Name;
        //pokemonLevel.text = $"Lv {pokemon.Level}";
        SetLevelText();
        healthbar.SetHP((float)_pokemon.Hp / _pokemon.MaxHp);
        SetExp();//escalar la barra de experiencia nada mas cargar
        StartCoroutine( UpdatePokemonData(pokemon.Hp));
    }

    public IEnumerator UpdatePokemonData(int olHPval)
    {
        StartCoroutine( healthbar.SetSmoothHP((float)_pokemon.Hp / _pokemon.MaxHp));
        StartCoroutine( DecraseHealthPoints(olHPval));
        yield return null;
    }

    private IEnumerator DecraseHealthPoints(int oldHPval)
    {
        while (oldHPval > _pokemon.Hp)
        {
            oldHPval--;
            pokemonHealth.text = $"{oldHPval} / {_pokemon.MaxHp}";
            yield return new WaitForSeconds(0.1f);
        }
        pokemonHealth.text = $"{_pokemon.Hp} / {_pokemon.MaxHp}";
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
}
