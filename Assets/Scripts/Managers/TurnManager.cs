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

    public static bool TurnButtonActive = true;

    public static GameManager gm;

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
        CurrentSelected = null;
        CurrentTeam = null;

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
        PrepareTeam();
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

        // fix bug later
        if (TeamUnits[toRemove.tag].Contains(toRemove))
        {
            TeamUnits[toRemove.tag].Remove(toRemove);
            if (TeamUnits[toRemove.tag].Count == 0)
            {
                TeamUnits.Remove(toRemove.tag);

                if (toRemove.tag == CurrentTeam)
                {
                    TeamQueue.Dequeue();
                }
                else
                {
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
            }
            if (CurrentSelected == toRemove)
            {
                EndTurn();
            }
        }
    }

    public static void PrepareTeam()
    {
        foreach (TacticsMove unit in TeamUnits[CurrentTeam].ToArray())
        {
            unit.charChar.SetActionState(Actions.Move, false);
            unit.charChar.SetActionState(Actions.Attack, false);
            unit.passedTurn = false;
            unit.distanceMoved = 0;
            unit.charChar.StartTurn();
        }

        StartTurn();
    }

    public static void NextTeam()
    {
        if (TeamUnits.ContainsKey(CurrentTeam))
        {
            foreach (TacticsMove unit in TeamUnits[CurrentTeam].ToArray())
            {
                unit.charChar.EndTurn();
                unit.charChar.SetActionState(Actions.Move, true);
                unit.charChar.SetActionState(Actions.Attack, true);
            }
        }

        if (CurrentTeam == "Player") TurnButtonActive = false;

        if (TeamQueue.Contains(CurrentTeam))
        {
            TeamQueue.Enqueue(TeamQueue.Dequeue());
        }
        else if (TeamQueue.Count == 0) {
            EndCombat();
            return;
        }

        CurrentTeam = TeamQueue.Peek();
        CurrentSelected = TeamUnits[CurrentTeam][0];

        PrepareTeam();

        if (CurrentTeam == "Player") TurnButtonActive = true;
    }

    public static void StartTurn()
    {
        CurrentSelected.BeginTurn();
        CurrentSelected.MoveAction();
    }

    public static void EndTurn()
    {
        CurrentSelected.passedTurn = true;
        CurrentSelected.EndTurn();

        if (TeamUnits.ContainsKey(CurrentTeam))
        {
            foreach (TacticsMove unit in TeamUnits[CurrentTeam].ToArray())
            {
                if (!unit.passedTurn)
                {
                    CurrentSelected = unit;
                    StartTurn();
                    return;
                }
            }
        }

        foreach (Actions action in CurrentSelected.charChar.ActionCost.Keys.ToArray())
        {
            CurrentSelected.charChar.SetActionState(action, true);
        }

        NextTeam();
    }
}
