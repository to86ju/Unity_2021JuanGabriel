using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Pokemon",menuName ="Pokemon/New Pokemon")]

public class PokemonBase : ScriptableObject
{
    [SerializeField] private int ID;
    [SerializeField] private string name;
   
    [TextArea] [SerializeField] private string description;
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private PokemonType type1, type2;

    //Stats
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;

    public string Name => name;
    public string Description => description;
    public PokemonType Type1 => type1;
    public PokemonType Tipe2 => type2;
    public int MaxHP => maxHP;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;


    [SerializeField] private List<LearnableMove> learnebleMoves;

    public List<LearnableMove> LearnableMoves => learnebleMoves;

    public Sprite FrontSprite { get => frontSprite; }
    public Sprite BackSprite { get => backSprite;  }
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Fight,
    Ice,
    Poison,
    Ground,
    Fly,
    Psychic,
    Rock,
    Bug,
    Ghost,
    Dragon,
    Dark,
    Fairy,
    Steel
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase  move;
    [SerializeField] private int level;

    public MoveBase Move => move;
    public int Level => level;
}
