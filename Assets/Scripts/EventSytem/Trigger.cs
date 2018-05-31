using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public class Trigger : MonoBehaviour
{
    public List<List<object[]>> triggerList = new List<List<object[]>>();
    public List<StatusEffect> statusEffect = new List<StatusEffect>();

    public Damage newDamage;

    public void CreateTrigger(string targetTriggerList, object[] trigger)
    {
        foreach (List<object[]> tL in triggerList)
        {
            if ((string)tL[0][0] == targetTriggerList)
            {
                tL.Add(trigger);
                return;
            }
        }
        triggerList.Add(new List<object[]> { new object[] { targetTriggerList } });
        CreateTrigger(targetTriggerList, trigger);
    }

    public void CallCombatEvents(string eventName)
    {
        EventTrigger(newDamage.actor, newDamage.triggerList, ("atk_" + eventName));
        EventTrigger(newDamage.target, newDamage.target.triggerList, ("def_" + eventName));
    }

    public void EventTrigger(Character target, List<List<object[]>> eventListList, string eventName)
    {
        foreach (List<object[]> eventList in eventListList)
        {
            if ((string)eventList[0][0] == eventName)
            {
                for (int a = 1; a <= (eventList.Count - 1); a++)
                {
                    object[] arguments = new object[eventList[a].Length - 1];
                    for (int i = 1; i < eventList[a].Length; i++)
                    {
                        arguments[i - 1] = eventList[a][i];
                    }
                    if (target.GetType().GetMethod((string)eventList[a][0]) != null)
                    {
                        InvokeStringMethod(target, (string)eventList[a][0], arguments);
                    }
                    else
                    {
                        EventTrigger(target, eventListList, (string)eventList[a][0]);
                        //CallCombatEvents((string)eventList[a][0]);
                    }
                }

            }
        }
    }

    public void InvokeStringMethod(Character target, string methodName, object[] args)
    {
        MethodInfo methodInfo = target.GetType().GetMethod(methodName);
        methodInfo.Invoke(target, args);
    }

    public object VarParser(object variable)
    {
        switch ((string)variable)
        {
            default: return variable;
        }
    }
}
