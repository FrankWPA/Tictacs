using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMove : TacticsMove
{
    GameObject targetUnit;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (!turn)
        {
            return;
        }
        else if (currentMoveType == MoveType.Selection)
        {
            if (!moving)
            {
                FindNearestTarget();
                CalculatePath();
                FindSelectableTiles();
            }
            else
            {
                MoveSelection();
            }
        }
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
