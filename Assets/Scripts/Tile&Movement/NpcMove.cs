using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMove : TacticsMove
{

    void Start()
    {
        Init();
		moveDistance = this.GetComponent<EnemyCharacter> ().movement;
    }

    void Update()
    {
        if (turn)
        {
            if (!charChar.GetActionState(Actions.Move))
            {
                if (moving)
                {
                    MoveSelection();
                }
                else
                {
                    MoveAction();
                }
            }

            else if (!charChar.GetActionState(Actions.Attack))
            {
                charChar.CauseDamage();
                TurnManager.EndTurn();
            }
        }
    }

    public override void MoveAction()
    {
        FindNearestTarget();
        if (targetUnit != null)
        {
            CalculatePath();
        }
        else
        {
            charChar.SetActionState(Actions.Move, true);
        }
        //FindSelectableTiles(1);
    }

    void CalculatePath()
    {
        Tiles targetTile = GetTargetTile(targetUnit);
        if (!FindPath(targetTile))
        {
            charChar.SetActionState(Actions.Move, true);
        }
    }

    public override void FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach (GameObject obj in targets)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);

            if (d < distance)
            {
                distance = d;
                nearest = obj;
            }
        }
        targetUnit = nearest;
    }
}
