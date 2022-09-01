using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{

    private PokemonBase _base;

    private int _level;

    private List<Move> _move;

    //Vida actual de pokemon
    private int _hp;

 
    public Pokemon(PokemonBase pokemonbase, int pokemonLevel)
    {
        _base = pokemonbase;
        _level = pokemonLevel;

        _hp = MaxHp;

        _move = new List<Move>();

        foreach (var lmove in _base.LearnableMoves)
        {
            if (lmove.Level <= _level)
            {
                _move.Add(new Move(lmove.Move));
            }

            if (_move.Count >= 4)
            {
                break;
            }
        }
    }

    public int MaxHp => Mathf.FloorToInt((_base.MaxHP * _level) / 20.0f) + 10;
    public int Attack => Mathf.FloorToInt((_base.Attack * _level) / 100.0f) + 1;
    public int Defense => Mathf.FloorToInt((_base.Defense * _level) / 100.0f) + 1;
    public int SpAttack => Mathf.FloorToInt((_base.SpAttack * _level) / 100.0f) + 1;
    public int SpDefense => Mathf.FloorToInt((_base.SpDefense * _level) / 100.0f) + 1;
    public int Speed => Mathf.FloorToInt((_base.Speed * _level) / 100.0f) + 1;

    public List<Move> Move { get => _move; set => _move = value; }
    public int Hp { get => _hp; set => _hp = value; }
    public PokemonBase Base { get => _base;  }
    public int Level { get => _level; }
}
