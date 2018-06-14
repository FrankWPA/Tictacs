using System.Collections.Generic;
using System.Linq;

public static class TurnManager
{
	public static Dictionary<string, List<TacticsMove>> TeamUnits = new Dictionary<string, List<TacticsMove>>();
    public static List<TacticsMove> UnitList = new List<TacticsMove>();
    public static Queue<string> TeamQueue = new Queue<string>();
    public static TacticsMove CurrentTurn;
    public static string CurrentTeam;
    
    public static void InitCombat()
    {
        InitTeamTurnQueue();
    }

    public static void EndCombat()
    {
        TeamUnits.Clear();
        TeamQueue.Clear();
    }

    static void InitTeamTurnQueue()
    {
        foreach (TacticsMove unit in UnitList)
        {
            if (TeamUnits.ContainsKey(unit.tag))
            {
                TeamUnits[unit.tag].Add(unit);
            }
            else
            {
                TeamUnits.Add(unit.tag, new List<TacticsMove> { unit });
            }
        }
        foreach (string team in TeamUnits.Keys)
        {
            TeamQueue.Enqueue(team);
        }
        CurrentTeam = TeamQueue.Peek();
        CurrentTurn = TeamUnits[CurrentTeam][0];
        StartTurn();
    }

    public static void NextTeam()
    {
        TeamQueue.Enqueue(TeamQueue.Dequeue());
        CurrentTeam = TeamQueue.Peek();
        CurrentTurn = TeamUnits[CurrentTeam][0];
    }

    public static void StartTurn()
    {
        CurrentTurn.passedTurn = false;
        CurrentTurn.BeginTurn();
        CurrentTurn.GetComponent<TacticsMove>().MoveAction();
    }

    public static void EndTurn()
    {
        CurrentTurn.distanceMoved = 0;
        CurrentTurn.passedTurn = true;
        CurrentTurn.EndTurn();

        foreach (TacticsMove.TurnActions action in CurrentTurn.ActionUse.Keys.ToArray())
        {
            CurrentTurn.ActionUse[action] = false;  
        }

        foreach (TacticsMove unit in TeamUnits[CurrentTeam].ToArray())
        {
            if (!unit.passedTurn)
            {
                CurrentTurn = unit;
                StartTurn();
                return;
            }
        }
        foreach (TacticsMove unit in TeamUnits[CurrentTeam].ToArray())
        {
            unit.passedTurn = false;
        }

        NextTeam();
        StartTurn();
    }

    public static void AddUnit(this TacticsMove toAdd)
    {
        UnitList.Add(toAdd);
    }

    public static void RemoveUnit(this TacticsMove toRemove)
    {
        UnitList.Remove(toRemove);

        if (TeamUnits[toRemove.tag].Contains(toRemove))
        {
            TeamUnits[toRemove.tag].Remove(toRemove);
            if (TeamUnits[toRemove.tag].Count == 0)
            {
                TeamUnits.Remove(toRemove.tag);

                while (TeamQueue.Peek() != toRemove.tag)
                {
                    TeamQueue.Enqueue(TeamQueue.Dequeue());
                }

                TeamQueue.Dequeue();

                while (TeamQueue.Peek() != CurrentTeam)
                {
                    TeamQueue.Enqueue(TeamQueue.Dequeue());
                }
            }
            if (CurrentTurn == toRemove)
            {
                NextTeam();
                EndTurn();
            }       
        }
    }
}
