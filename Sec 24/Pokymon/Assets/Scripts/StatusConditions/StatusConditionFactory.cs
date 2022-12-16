using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            }
        };

    private static void PoisonEffect(Pokemon pokemon)
    {
        pokemon.UpdateHP(pokemon.MaxHp/8);
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sufre los efectos del veneno");
    }
}

public enum StatusConditionId
{
    none, brn, frz, par, psn, slp
}
