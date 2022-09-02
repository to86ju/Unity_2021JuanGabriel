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
        UpdatePokemonData();
    }

    public void UpdatePokemonData()
    {
        StartCoroutine( healthbar.SetSmoothHP((float)_pokemon.Hp / _pokemon.MaxHp));
        pokemonHealth.text = $"{_pokemon.Hp} / {_pokemon.MaxHp}";
    }
}
