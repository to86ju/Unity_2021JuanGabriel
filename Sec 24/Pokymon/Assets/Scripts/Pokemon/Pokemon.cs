using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{

    private PokemonBase _base;

    private int _level;


    public Pokemon(PokemonBase pokemonbase, int pokemonLevel)
    {
        _base = pokemonbase;
        _level = pokemonLevel;
    }

    public int MaxHp => Mathf.FloorToInt((_base.MaxHP * _level) / 100.0f) + 10;
    public int Attack => Mathf.FloorToInt((_base.Attack * _level) / 100.0f) + 1;
    public int Defense => Mathf.FloorToInt((_base.Defense * _level) / 100.0f) + 1;
    public int SpAttack => Mathf.FloorToInt((_base.SpAttack * _level) / 100.0f) + 1;
    public int SpDefense => Mathf.FloorToInt((_base.SpDefense * _level) / 100.0f) + 1;
    public int Speed => Mathf.FloorToInt((_base.Speed * _level) / 100.0f) + 1;

}
