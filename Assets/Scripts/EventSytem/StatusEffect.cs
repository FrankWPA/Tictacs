using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventSystem.Trigger;

public class StatusEffect
{
    public List<object[]> StatusTriggerList = new List<object[]>();
    public EffectType type = EffectType.Null;

    public Character Owner = null;
    public int TurnDuration = -1;

    public void TurnUpdate()
    {
        if (TurnDuration !=  -1)
        {
            if (TurnDuration > 0)
            {
                TurnDuration--;
            }
            else
            {
                Owner.StatusEffectList.Remove(this);
                if (StatusTriggerList.Count > 0)
                {
                    RemoveTrigger();
                    return;
                }
            }
        }
        
        // Turn Related Status Stuff

    }

    // Trigger Related Stuff
    

    public void ApplyTrigger()
    {
        StatusTriggerList.ApplyTrigger(Owner);
    }

    public void RemoveTrigger()
    {
        Debug.Log("Removing Status");
        StatusTriggerList.RemoveTrigger(Owner);
    }
}

public enum EffectType
{
    Null, Buff, Debuff, Poison 
}