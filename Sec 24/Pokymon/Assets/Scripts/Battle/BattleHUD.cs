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

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        pokemonName.text = pokemon.Base.name;
        pokemonLevel.text = $"Lv {pokemon.Level}";
        healthbar.SetHP((float)_pokemon.Hp / _pokemon.MaxHp);
        UpdatePokemonData(pokemon.Hp);
    }

    public void UpdatePokemonData(int olHPval)
    {
        StartCoroutine( healthbar.SetSmoothHP((float)_pokemon.Hp / _pokemon.MaxHp));
        StartCoroutine(DecraseHealthPoints(olHPval));
        
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
}
