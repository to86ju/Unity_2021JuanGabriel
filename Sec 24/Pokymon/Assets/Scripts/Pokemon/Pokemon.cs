using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;

[Serializable]
public class Pokemon
{

    [SerializeField] private PokemonBase _base;

    [SerializeField] private int _level;

    private List<Move> _move;

    //Vida actual de pokemon
    private int _hp;

 
    public void  InitPokemon()
    {

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

    //Propiedades
    public int MaxHp => Mathf.FloorToInt((_base.MaxHP * _level) / 20.0f) + 10;
    public int Attack => Mathf.FloorToInt((_base.Attack * _level) / 100.0f) + 1;
    public int Defense => Mathf.FloorToInt((_base.Defense * _level) / 100.0f) + 1;
    public int SpAttack => Mathf.FloorToInt((_base.SpAttack * _level) / 100.0f) + 1;
    public int SpDefense => Mathf.FloorToInt((_base.SpDefense * _level) / 100.0f) + 1;
    public int Speed => Mathf.FloorToInt((_base.Speed * _level) / 100.0f) + 1;

    //Aciones
    public List<Move> Move { get => _move; set => _move = value; }
    public int Hp { get => _hp; set => _hp = value; }
    public PokemonBase Base { get => _base;  }
    public int Level { get => _level; }

    //Ataque
    public DameDescription ReceiveDamage(Pokemon attacker,Move move)
    {
        float critical = 1f;

        if (Random.Range(0f,100f) < 8f)
        {
            critical = 2f;
        }

        float type1 = TypeMatrix.GetMulttEffectiveness(move.Base.Type, this.Base.Type1);
        float type2 = TypeMatrix.GetMulttEffectiveness(move.Base.Type, this.Base.Type2);
        float types = type1 * type2;

        var damageDesc = new DameDescription()
        {
            Critical = critical,
            Type = types,
            Fainted = false
        };

        float attack = (move.Base.IsSpecialMove ? attacker.SpAttack : attacker.Attack);
        float defense = (move.Base.IsSpecialMove ? this.SpDefense : this.Defense);

        float modifierss = Random.Range(0.85f, 1.0f)* types * critical;
        float baseDamage = ((2 * attacker.Level / 5f + 2) * move.Base.Power * (attack/ (float)defense)) / 50f + 2;
        int totalDamage = Mathf.FloorToInt(baseDamage * modifierss);

        Hp -= totalDamage;

        if (Hp <= 0)
        {
            Hp = 0;
            damageDesc.Fainted = true;
        }

        return damageDesc;
    }

    //ataque random del enemigo
    public Move RandomMove()
    {
        var movesWithPP = Move.Where(m => m.Pp > 0).ToList();

        if (movesWithPP.Count >0)
        {
            int randId = Random.Range(0, movesWithPP.Count);

            return movesWithPP[randId];
        }

        //No hay pps en nigun ataque
        //TODO: Implementar combate, que hace daño al enemigo y a ti mismo
        return null;
        
    }
}

public class DameDescription
{
    public float Critical { get; set; }
    public float Type { get; set; }
    public bool Fainted { get; set; }
}
