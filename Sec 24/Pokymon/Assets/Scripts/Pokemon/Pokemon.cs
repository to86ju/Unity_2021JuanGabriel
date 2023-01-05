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

    public bool HasHPChanged { get; set; }
    public int previousHPValue;

    public Pokemon(PokemonBase pBase, int plevel)
    {
        _base = pBase;
        _level = plevel;

        InitPokemon();
    }


    public void InitPokemon()
    {

        _experience = Base.GetNecessaryExpForLevel(_level);//iniciar la experiencia 

        //----------- lista de move ------------------
        _move = new List<Move>();

        foreach (var lmove in _base.LearnableMoves)
        {
            if (lmove.Level <= _level)
            {
                _move.Add(new Move(lmove.Move));
            }

            if (_move.Count >= PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
            {
                break;
            }
        }
        //---------------------------------------------

        CalculateStats();
        _hp = MaxHp;
        previousHPValue = MaxHp;
        HasHPChanged = true;

        ResetBoostings();
        statusCodition = null;
        VolatilestatusCodition = null;
    }


    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();

        Stats.Add(Stat.Atack, Mathf.FloorToInt((_base.Attack * _level) / 100.0f) + 2);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((_base.Defense * _level) / 100.0f) + 2);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((_base.SpAttack * _level) / 100.0f) + 2);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((_base.SpDefense * _level) / 100.0f) + 2);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((_base.Speed * _level) / 100.0f) + 2);

        MaxHp = Mathf.FloorToInt((_base.MaxHP * _level)/20.0f)+10+_level;
    }

    private void ResetBoostings()
    {
        StatusChangeMessages = new Queue<string>();

        StatsBoosted = new Dictionary<Stat, int>()
        {
            {Stat.Atack, 0 },
            {Stat.Defense, 0 },
            {Stat.SpAttack, 0 },
            {Stat.SpDefense, 0 },
            {Stat.Speed, 0 },
            {Stat.Accuracy, 0 },
            {Stat.Evasion, 0 }
        };
    }

    private int GetStat(Stat stat)
    {
        int statValue = Stats[stat];

        int boost = StatsBoosted[stat];// esdes -6 hasta 6

        float mulptiplier = 1.0f + Mathf.Abs(boost) / 2.0f;

        // 1 -> 1.5 -> 2 -> 2.5 -> ... ->4
        if (boost >= 0)
        {
            
            statValue = Mathf.FloorToInt(statValue * mulptiplier);
        }
        else
        {
            
            statValue = Mathf.FloorToInt(statValue / mulptiplier);
        }

        return statValue;
    }

    public void ApplyBoost(StatBoosting boost)
    {
        
        var stat = boost.stat;
        var value = boost.boost;

        StatsBoosted[stat] =  Mathf.Clamp( StatsBoosted[stat] + value,-6, 6);

        if (value > 0)
        {
            StatusChangeMessages.Enqueue($"{Base.name} ha incrementado su {stat}");
        }

        else if(value < 0)
        {
            StatusChangeMessages.Enqueue($"{Base.name} ha reducido su {stat}");        
        }

        else
        {
            StatusChangeMessages.Enqueue($"{Base.name} no nota ningún efecto");
        }

    }

    //Propiedades
    public int MaxHp {get; private set;}
    public int Attack => GetStat(Stat.Atack);
    public int Defense => GetStat(Stat.Defense);
    public int SpAttack => GetStat(Stat.SpAttack);
    public int SpDefense => GetStat(Stat.SpDefense);
    public int Speed => GetStat(Stat.Speed);
    private int _experience;

    //Aciones
    public List<Move> Moves { get => _move; set => _move = value; }


    //extadisticas
    public Dictionary<Stat, int> Stats { get; private set; }

    public Dictionary<Stat, int> StatsBoosted { get; private set; }

    public StatusCondition statusCodition { get; set; }
    public int StatusNumTurns { get; set; }

    public StatusCondition VolatilestatusCodition { get; set; }
    public int VolatilesStatusNumTurns { get; set; }

    public Queue<string> StatusChangeMessages { get; private set; } = new Queue<string>(); //cola

    public event Action OnStatusConditionChanged;


    public int Hp { get => _hp; set { _hp = value; _hp = Mathf.FloorToInt( Mathf.Clamp(_hp, 0, MaxHp)); } }
    public int Experience { get => _experience; set => _experience = value; }

    
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

        UpdateHP(totalDamage);

        if (Hp <=0)
        {
            damageDesc.Fainted = true;
        }

        return damageDesc;
    }

    public void UpdateHP(int damage)
    {
        HasHPChanged = true;
        previousHPValue = Hp;
        Hp -= damage;

        if (Hp <= 0)
        {
            Hp = 0;
        }
    }


    //Metodo Estados alterados
    public void SetConditionStatus(StatusConditionId id)
    {
        if (statusCodition != null)
        {
            return;
        }
        statusCodition = StatusConditionFactory.StatusConditions[id];
        statusCodition?.OnApplyStatusCondition?.Invoke(this);//estado alterado el dormido
        StatusChangeMessages.Enqueue($"{Base.Name} {statusCodition.StartMessage}");
        OnStatusConditionChanged?.Invoke();
    }

    public void SetVolatileConditionStatus(StatusConditionId id)
    {
        if (VolatilestatusCodition != null)
        {
            return;
        }
        VolatilestatusCodition = StatusConditionFactory.StatusConditions[id];
        VolatilestatusCodition?.OnApplyStatusCondition?.Invoke(this);//estado alterado el dormido
        StatusChangeMessages.Enqueue($"{Base.Name} {VolatilestatusCodition.StartMessage}");
        
    }

    public bool OnStartTurn()
    {
        bool canPerforMovement = true;

        if (statusCodition?.OnStartTurn != null)
        {
            if (!statusCodition.OnStartTurn(this))
            {
                canPerforMovement = false;
            }
        }

        if (VolatilestatusCodition?.OnStartTurn != null)
        {
            if (!VolatilestatusCodition.OnStartTurn(this))
            {
                canPerforMovement = false;
            }
        }

        return canPerforMovement;
    }

    //ataque random del enemigo
    public Move RandomMove()
    {
        var movesWithPP = Moves.Where(m => m.Pp > 0).ToList();

        if (movesWithPP.Count >0)
        {
            int randId = Random.Range(0, movesWithPP.Count);

            return movesWithPP[randId];
        }

        //No hay pps en nigun ataque
        //TODO: Implementar combate, que hace daño al enemigo y a ti mismo
        return null;
        
    }

    public bool NeedsToLevelUp()
    {
        if (Experience > Base.GetNecessaryExpForLevel(_level+1))
        {
            int currentMaxHp = MaxHp;
            _level++;
            Hp += (MaxHp - currentMaxHp);
            return true;
        }
        return false;
    }

    public LearnableMove GetLearnablemOveAtCurentLevel()
    {
        return Base.LearnableMoves.Where(lm => lm.Level == _level).FirstOrDefault();
    }

    public void CureStatusCondition()
    {
        statusCodition = null;
        OnStatusConditionChanged?.Invoke();
    }

    public void CureVolatilesStatusCondition()
    {
        VolatilestatusCodition = null;
    }


    public void learMove(LearnableMove learnableMove)
    {
        if (Moves.Count >= PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
        {
            return;
        }

        Moves.Add(new Move(learnableMove.Move));
    }

    public void OnBattleFinish()
    {
        VolatilestatusCodition = null;
        ResetBoostings();
    }

    public void OnFinishTurn()
    {
        statusCodition?.OnFinishTur?.Invoke(this);
        VolatilestatusCodition?.OnFinishTur?.Invoke(this);
    }
}

public class DameDescription
{
    public float Critical { get; set; }
    public float Type { get; set; }
    public bool Fainted { get; set; }
}


