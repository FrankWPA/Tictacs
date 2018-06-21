using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMove : TacticsMove
{
    public GameObject targetUnit;

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

            if (!charChar.GetActionState(Actions.Attack))
            {
                charChar.CauseDamage(charChar.damageTarget);
                charChar.SetActionState(Actions.Attack, true);
            }
        }
    }

    public override void MoveAction()
    {
        FindNearestTarget();
        CalculatePath();
        //FindSelectableTiles(1);
    }

    void CalculatePath()
    {
        Tiles targetTile = GetTargetTile(targetUnit);
        FindPath(targetTile);
    }

    void FindNearestTarget()
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
