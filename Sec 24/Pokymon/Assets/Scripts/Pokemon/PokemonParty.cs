using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;
    public const int NUM_MAX_POKEMON_IN_PARTY = 6;

    public List<Pokemon> Pokemons 
    { 
        get => pokemons; 
        set => pokemons = value; 
    }

    private void Start()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.InitPokemon();            
        }
    }

    //Metodo que te da el primer pokemon sano
    public Pokemon GetFirstNonFaintedPokemon()
    {
        //Debuelve el primer komemon con vida
        return pokemons.Where(p => p.Hp > 0).FirstOrDefault();
    }

    public int GetPositionFromPokemon(Pokemon pokemon)
    {
        // ------ obtener la posicion del pokemon actual -----
        for (int i = 0; i < Pokemons.Count; i++)
        {
            if (pokemon == Pokemons[i])
            {
                return i;
            }
        }
        //-------------------------------------------------------
        return -1;
    }

    public bool AddPokemonToParty(Pokemon pokemon)
    {
        if (pokemons.Count < NUM_MAX_POKEMON_IN_PARTY)
        {
            pokemons.Add(pokemon);
            return true;
        }
        else
        {
            //Añadir la funcinalidad en enviar al pc de bill
            return false;
        }
    }
}
