using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Random = UnityEngine.Random;

public class StatusConditionFactory 
{
    public static void InitFactory()
    {
        foreach (var condition in StatusConditions)
        {
            var id = condition.Key;
            var statusCondition = condition.Value;

            statusCondition.Id = id;
        }
    }

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

            {
                StatusConditionId.slp,
                new StatusCondition()
                {
                    Name = "Sleep",
                    Descrption = "Hace que el Pokemon duerma durante un numero fijo de turnos",
                    StartMessage = "se ha dormido",

                    OnApplyStatusCondition = (Pokemon pokemon) =>
                    {
                        pokemon.StatusNumTurns = Random.Range(1,4);
                        Debug.Log($"El pokemon dormira durante {pokemon.StatusNumTurns} turnos");
                    },
                    OnStartTurn = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusNumTurns <=0)
                            {
                                pokemon.CureStatusCondition();
                                pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} ha despertado!");
                                return true;
	                        }
                        pokemon.StatusNumTurns --;
                        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sigue dormido.");
                        return false;
                    }
                    

                }
            },

            // ----------Estados Volatiles -----------------------
            {
                StatusConditionId.conf,
                new StatusCondition()
                {
                    Name = "Confusión",
                    Descrption = "Hace que el Pokemon esté confundido y pueda atcarse a si mismo",
                    StartMessage = "ha sido confundido",

                    OnApplyStatusCondition = (Pokemon pokemon) =>
                    {
                        pokemon.VolatilesStatusNumTurns = Random.Range(1,6);
                        Debug.Log($"El pokemon estara confundido {pokemon.VolatilesStatusNumTurns} turnos");
                    },
                    OnStartTurn = (Pokemon pokemon) =>
                    {
                        if (pokemon.VolatilesStatusNumTurns <=0)
                            {
                                pokemon.CureVolatilesStatusCondition();
                                pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} ha salido del estado confusion");
                                return true;
                            }
                        pokemon.VolatilesStatusNumTurns --;

                        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sigue confundido.");

                        if (Random.Range(0,2) == 0)
                        {
                            return true;
                        }

                        //debemos dañarnos a nostros mismos por la confusión
                        pokemon.UpdateHP(pokemon.MaxHp/6);

                        pokemon.StatusChangeMessages.Enqueue("Tan cofuso que se hire a si mismo");

                        return false;
                    }


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
    none, brn, frz, par, psn, slp, conf
}
