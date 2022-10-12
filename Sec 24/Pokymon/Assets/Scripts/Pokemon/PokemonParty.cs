using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

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
}
