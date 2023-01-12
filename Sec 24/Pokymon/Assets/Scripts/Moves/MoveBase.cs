using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Move",menuName ="Pokemon/Nuevo Movimiento")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private bool alwaysHIt;
    [SerializeField] private int pp;
    [SerializeField] private int priority;
    [SerializeField] private MoveType moveType;
    [SerializeField] private MoveStatEffect effects;
    [SerializeField] private List<SecondaryMoveStatEffect> secondaryEffects;
    [SerializeField] private MoveTarget target;

    public string Name => name;
    public string Descrption => description;
    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public bool Alwayshit => alwaysHIt;
    public int Pp => pp;
    public int Priority => priority;
    public MoveType MoveType => moveType;

    public MoveStatEffect Effects => effects;
    public List<SecondaryMoveStatEffect> SecondaryEfects => secondaryEffects; 
    public MoveTarget Target => target;

    public bool IsSpecialMove => moveType == MoveType.Special;

     
    /*if (type == PokemonType.Fire || type == PokemonType.Water || 
    type == PokemonType.Grass|| type == PokemonType.Ice || 
    type == PokemonType.Electric || type == PokemonType.Dragon || 
    type == PokemonType.Dark || type == PokemonType.Psychic)
    {
        return true;
    }
    else
    {
        return false;
    }
    */

}

public enum MoveType
{
    Phisical,
    Special,
    Stats
}

[System.Serializable]
public class MoveStatEffect
{
    [SerializeField] private List<StatBoosting> boostings;
    [SerializeField] private StatusConditionId status;
    [SerializeField] private StatusConditionId volatileStatus;
    public List<StatBoosting> Boostings => boostings;

    public StatusConditionId Status => status;
    public StatusConditionId VolatilesStatus => volatileStatus;
}

[System.Serializable]
public class SecondaryMoveStatEffect : MoveStatEffect
{
    [SerializeField] private int chance;

    [SerializeField] private MoveTarget target;

    public int Chance { get => chance; }
    public MoveTarget Target { get => target; }
}

[System.Serializable]
public class StatBoosting
{
    public Stat stat;
    public int boost;
    public MoveTarget target;
}

public enum MoveTarget
{
    Self, Other
}
