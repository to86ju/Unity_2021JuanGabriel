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

    [SerializeField] private int catchRate= 255;

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
    public PokemonType Type2 => type2;
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
    public int CatchRate { get => catchRate; set => catchRate = value; }
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fight,
    Poison,
    Ground,
    Fly,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public class TypeMatrix
{
    private static float[][] matrix =
    {
        //                  NOR  FIR  WAT  ELE  GRA  ICE  FIG  POI  GRO  FLY  PSY  BUG  ROC  GHO  DRA  DAR  STE  FAI
        /*NOR*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,0.5f,0f  ,1f  ,1f  ,0.5f,1f  },
        /*FIR*/ new float[]{1f  ,0.5f,0.5f,1f  ,2f  ,2f  ,1f  ,1f  ,1f  ,1f  ,1f  ,2f  ,0.5f,1f  ,0.5f,1f  ,2f  ,1f  },
        /*WAT*/ new float[]{1f  ,2f  ,0.5f,1f  ,0.5f,1f  ,1f  ,1f  ,2f  ,1f  ,1f  ,1f  ,2f  ,1f  ,0.5f,1f  ,1f  ,1f  },
        /*ELE*/ new float[]{1f  ,1f  ,2f  ,0.5f,0.5f,1f  ,1f  ,1f  ,0f  ,2f  ,1f  ,1f  ,1f  ,1f  ,0.5f,1f  ,1f  ,1f  },
        /*GRA*/ new float[]{1f  ,0.5f,2f  ,1f  ,0.5f,1f  ,1f  ,0.5f,2f  ,0.5f,1f  ,0.5f,2f  ,1f  ,0.5f,1f  ,0.5f,1f  },
        /*ICE*/ new float[]{1f  ,0.5f,0.5f,1f  ,2f  ,0.5f,1f  ,1f  ,2f  ,2f  ,1f  ,1f  ,1f  ,1f  ,2f  ,1f  ,0.5f,1f  },
        /*FIG*/ new float[]{2f  ,1f  ,1f  ,1f  ,1f  ,2f  ,1f  ,0.5f,1f  ,0.5f,0.5f,0.5f,2f  ,0f  ,1f  ,2f  ,2f  ,0.5f},
        /*POI*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  },
        /*GRO*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  },
        /*FLY*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  },
        /*PSY*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  },
        /*BUG*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  },
        /*ROC*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  },
        /*GHO*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  },
        /*DRA*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  },
        /*DAR*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  },
        /*STE*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  },
        /*FAI*/ new float[]{1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  }
    };

    public static float GetMulttEffectiveness(PokemonType PokemonType, PokemonType PokemondefenderType)
    {
        if (PokemonType == PokemonType.None || PokemondefenderType == PokemonType.None)
        {
            return 1.0f;
        }

        int row = (int)PokemonType;
        int col = (int)PokemondefenderType;

        return matrix[row - 1][col - 1];
    }
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase  move;
    [SerializeField] private int level;

    public MoveBase Move => move;
    public int Level => level;
}
