using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text pokemonName;
    public Text pokemonLevel;
    public HealthBar healthbar;



    public void SetPokemonData(Pokemon pokemon)
    {
        pokemonName.text = pokemon.Base.name;
        pokemonLevel.text = $"Lv {pokemon.Level}";
        healthbar.SetHP(pokemon.Hp / pokemon.MaxHp);
    }
}
