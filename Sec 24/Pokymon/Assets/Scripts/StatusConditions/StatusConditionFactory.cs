using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Random = UnityEngine.Random;

public class StatusConditionFactory 
{
    public static Dictionary<StatusConditionId, StatusCondition> StatusConditions { get; set; } = 
        new Dictionary<StatusConditionId, StatusCondition>() 
        {
            {
                StatusConditionId.psn,
                new StatusCondition()
                {
                    Name = "Poison",
                    Descrption = "Hace que el Pokemon sufra daño en cada turno",
                    StartMessage = "ha sido envenenado",

                    OnFinishTur = PoisonEffect//accion

                }
            },

            {
                StatusConditionId.brn,
                new StatusCondition()
                {
                    Name = "Burn",
                    Descrption = "Hace que el Pokemon sufra daño en cada turno",
                    StartMessage = "ha sido quemado",

                    OnFinishTur = BurnEffect//accion

                }
            },

            {
                StatusConditionId.par,
                new StatusCondition()
                {
                    Name = "Paralyzed",
                    Descrption = "Hace que el Pokemon pueda estar paralizado en el turno.",
                    StartMessage = "ha sido paralizado",

                    OnStartTurn = ParalyzedEffect//accion con resultado

                }
            },

            {
                StatusConditionId.frz,
                new StatusCondition()
                {
                    Name = "Frozen",
                    Descrption = "Hace que el Pokemon este congelado, pero se puede curar aleatoriamente en un turno",
                    StartMessage = "ha sido congelado",

                    OnStartTurn = FrozenEffect//accion con resultado

                }
            },
        };

    private static bool FrozenEffect(Pokemon pokemon)
    {
        if (Random.Range(0, 100) < 25)
        {
            pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} ya no esta congelado.");
            return true;
        }

        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} Sigue congelado.");
        return false;
    }

    private static bool ParalyzedEffect(Pokemon pokemon)
    {
        if (Random.Range(0,100) < 25)
        {
            pokemon.CureStatusCondition();
            pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} está paralizado y no puede moverse.");
            return false;
        }
        return true;
    }

    private static void BurnEffect(Pokemon pokemon)
    {
        pokemon.UpdateHP(Mathf.CeilToInt((float)pokemon.MaxHp / 15.0f));
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sufre los efectos de la quemadura");
    }

    private static void PoisonEffect(Pokemon pokemon)
    {
        pokemon.UpdateHP(Mathf.CeilToInt((float) pokemon.MaxHp/8.0f));
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sufre los efectos del veneno");
    }
}

public enum StatusConditionId
{
    none, brn, frz, par, psn, slp
}
