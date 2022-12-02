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
                    Descrption = "Hace que el Pokemon sugra daño en cada turno",
                    StartMessage = "ha sido envenenado",

                }
            }
        };
}

public enum StatusConditionId
{
    none, brn, frz, par, psn, slp
}
