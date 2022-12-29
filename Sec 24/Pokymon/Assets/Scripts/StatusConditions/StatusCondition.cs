using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCondition 
{
    public StatusConditionId Id { get; set; }
    public string Name { get; set; }
    public string Descrption { get; set; }
    public string StartMessage { get; set; }

    public Action<Pokemon> OnApplyStatusCondition { get; set; }

    public Func<Pokemon, bool> OnStartTurn { get; set; }
    public Action<Pokemon> OnFinishTur { get; set; }
    
}
