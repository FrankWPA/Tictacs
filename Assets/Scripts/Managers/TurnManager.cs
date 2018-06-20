using System.Collections.Generic;
using System.Linq;

public static class TurnManager
{
	public static Dictionary<string, List<TacticsMove>> TeamUnits = new Dictionary<string, List<TacticsMove>>();
    public static List<TacticsMove> UnitList = new List<TacticsMove>();
    public static Queue<string> TeamQueue = new Queue<string>();
    public static TacticsMove CurrentSelected;
    public static string CurrentTeam;

    public static bool QueueReady = true;
    public static bool combatInitialized = false;
    
    public static void InitCombat()
    {
        if (!combatInitialized)
        {
            InitTeamTurnQueue();
            combatInitialized = true;
        }
    }

    public static void EndCombat()
    {
        TeamUnits.Clear();
        TeamQueue.Clear();
        combatInitialized = QueueReady = false;
    }

    static void InitTeamTurnQueue()
    {
        if (!QueueReady)
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

            QueueReady = true;
        }

        CurrentTeam = TeamQueue.Peek();
        CurrentSelected = TeamUnits[CurrentTeam][0];
        StartTurn();
    }

    public static void AddUnit(this TacticsMove toAdd)
    {
        UnitList.Add(toAdd);

        if (TeamUnits.ContainsKey(toAdd.tag))
        {
            TeamUnits[toAdd.tag].Add(toAdd);
        }
        else
        {
            TeamUnits.Add(toAdd.tag, new List<TacticsMove> { toAdd });
            TeamQueue.Enqueue(toAdd.tag);
        }
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
            if (CurrentSelected == toRemove)
            {
                NextTeam();
                EndTurn();
            }
        }
    }

    public static void NextTeam()
    {
        TeamQueue.Enqueue(TeamQueue.Dequeue());
        CurrentTeam = TeamQueue.Peek();
        CurrentSelected = TeamUnits[CurrentTeam][0];
    }

    public static void StartTurn()
    {
        CurrentSelected.passedTurn = false;
        CurrentSelected.BeginTurn();
        CurrentSelected.GetComponent<TacticsMove>().MoveAction();
    }

    public static void EndTurn()
    {
        CurrentSelected.distanceMoved = 0;
        CurrentSelected.passedTurn = true;
        CurrentSelected.EndTurn();

        foreach (TurnActions action in CurrentSelected.ActionUse.Keys.ToArray())
        {
            CurrentSelected.ActionUse[action] = false;  
        }

        foreach (TacticsMove unit in TeamUnits[CurrentTeam].ToArray())
        {
            if (!unit.passedTurn)
            {
                CurrentSelected = unit;
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
}
