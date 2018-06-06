using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
	public static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>();
	public static Queue<string> turnKey = new Queue<string>();
    public static Queue<TacticsMove> turnTeam = new Queue<TacticsMove>();
	public TacticsMove actualTurn;

    // Update is called once per frame
    void Update()
    {
        if (turnTeam.Count == 0)
        {
            InitTeamTurnQueue();
        }
		actualTurn = turnTeam.Peek ();
    }

    static void InitTeamTurnQueue()
    {
        List<TacticsMove> teamList = units[turnKey.Peek()]; ;
        foreach (TacticsMove unit in teamList)
        {
            Debug.Log("InitTeamTurnQueue");
            turnTeam.Enqueue(unit);
        }
        StartTurn();
    }

    public static void StartTurn()
    {
        if (turnTeam.Count > 0)
        {
            turnTeam.Peek().BeginTurn();
        }
    }

    public static void EndTurn()
    {
        TacticsMove unit = turnTeam.Dequeue();
        unit.EndTurn();

        if (turnTeam.Count > 0)
        {
            Debug.Log("EndTurn1");
            StartTurn();
        }
        else
        {
            Debug.Log("EndTurn2");
            string team = turnKey.Dequeue();
            turnKey.Enqueue(team);
            InitTeamTurnQueue();
        }
    }

    public static void AddUnit(TacticsMove unit)
    {
        
        List<TacticsMove> list;
        if (!units.ContainsKey(unit.tag))
        {
            list = new List<TacticsMove>();
            units[unit.tag] = list;

            if (!turnKey.Contains(unit.tag))
            {
                Debug.Log("AddUnit1");
                turnKey.Enqueue(unit.tag);
            }
            else
            {
                Debug.Log("AddUnit2");
            }
        }
        else
        {
            Debug.Log("AddUnit3");
            list = units[unit.tag];
        }
        list.Add(unit);
    }

    public static void RemoveUnit(TacticsMove toRemove)
    {
        foreach(List<TacticsMove> value in units.Values)
        {
            foreach (TacticsMove TM in value)
            {
                if (TM == toRemove)
                {
                    value.Remove(toRemove);
                }
            }
        }
    }

	public void EndTurnButton(){
		EndTurn ();
	}
}
