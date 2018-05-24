using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    public enum MoveType { Selection, Line }
    public MoveType currentMoveType = MoveType.Line;

    public bool turn = false;
    public bool moving = false;


    public float moveSpeed = 10.0f;
    public int moveDistance;
	public int distanceMoved;

    public Tiles currentTile;
    public List<Tiles> pathList = new List<Tiles>();

    List<Tiles> targetList = new List<Tiles>();
    public Stack<Tiles> path = new Stack<Tiles>();
    List<Tiles> selectableTiles = new List<Tiles>();

    Vector3 target = new Vector3();
    GameObject[] tiles;

    public float halfHeight;
    public Tiles actualTargetTile;
	public bool TilesChecked = false;

    public void Init()
    {
		TurnManager.AddUnit(this);
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfHeight = GetComponent<Collider>().bounds.extents.y;
    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    public Tiles GetTargetTile(GameObject targetPos)
    {
        RaycastHit hit;
        Tiles tile = null;
        if (Physics.Raycast(targetPos.transform.position, -Vector3.up, out hit, halfHeight))
        {
            tile = hit.collider.GetComponent<Tiles>();
        }
        return tile;
    }

	public void ComputeAdjacencyLists(Tiles targetC, int ResetAll)
    {
        foreach (GameObject tile in tiles)
        {
            Tiles t = tile.GetComponent<Tiles>();
			t.CheckNeighbours(this.gameObject, halfHeight, targetC, ResetAll);
        }
    }

	public void FindSelectableTiles(int ResetAll)
    {
		ComputeAdjacencyLists(null, ResetAll);
		if (ResetAll != 0) {
			GetCurrentTile ();
		}

        Queue<Tiles> process = new Queue<Tiles>();

        process.Enqueue(currentTile);
        currentTile.visited = true;

        while (process.Count > 0)
        {
            Tiles t = process.Dequeue();

            selectableTiles.Add(t);
			if (ResetAll != 0) {
				t.selectable = true;
			}

			if (t.distance < (moveDistance - distanceMoved) || ResetAll == 0)
            {
                foreach (Tiles tile in t.adjacencyList)
                {
                    
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                        if (!tile.adjacencyList.Contains(tile.parent))
                        {
                            tile.adjacencyList.Add(tile.parent);
                        }
                    }
                }
            }
        }
    }

    public void MoveLine()
    {
        if (pathList.Count != 1)
        {
            Tiles t = targetList[1];
            target = t.transform.position;
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;
        }
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if (transform.position == target)
        {
            if (pathList.Count == 1)
            {
                moving = false;
                pathList.RemoveAt(0);
				if ((moveDistance - distanceMoved) == 0)
                	TurnManager.EndTurn();
            }
            else
            {
				distanceMoved++;
                pathList.RemoveAt(1);
            }
        }
    }

    public void MoveSelection()
    {
        if (path.Count > 0)
        {
            Tiles t = path.Peek();
            Vector3 target = t.transform.position;
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            if (transform.position == target)
            {
				distanceMoved++;
                path.Pop();
            }
        }
        else
        {
            moving = false;
			if ((moveDistance - distanceMoved) <= 0)
				TurnManager.EndTurn();
        }
    }

    public void MoveToTile(Tiles t)
    {
        path.Clear();
        t.target = true;
        moving = true;

        Tiles next = t;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    public void MoveToTile(List<Tiles> t)
    {
        moving = true;
        targetList = t;
    }

    protected void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (Tiles tile in selectableTiles)
        {
            tile.Reset(1);
        }

        selectableTiles.Clear();
    }

    public Tiles FindLowestF(List<Tiles> list)
    {
        Tiles lowest = list[0];
        foreach (Tiles t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }
        list.Remove(lowest);
        return lowest;
    }

    protected Tiles FindEndTile(Tiles t)
    {
        Stack<Tiles> tempPath = new Stack<Tiles>();

        Tiles next = t.parent;
        while (next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }
		if (tempPath.Count <= (moveDistance - distanceMoved))
        {
            return t.parent;
        }
        Tiles endTile = null;
		for (int i =0; i <= (moveDistance - distanceMoved); i++)
        {
            endTile = tempPath.Pop();
        }

        return endTile;
    }

    protected void FindPath(Tiles targetF)
    {
        ComputeAdjacencyLists(targetF, 1);
        GetCurrentTile();

        List<Tiles> openList = new List<Tiles>();
        List<Tiles> closedList = new List<Tiles>();

        openList.Add(currentTile);
        //currentTile.parent = ??;
        currentTile.h = Vector3.Distance(currentTile.transform.position, targetF.transform.position);
        currentTile.f = currentTile.h;

        while (openList.Count > 0)
        {
            Tiles t = FindLowestF(openList);
            closedList.Add(t);
            if (t == targetF)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                return;
            }

            foreach(Tiles tile in t.adjacencyList)
            {
                if (closedList.Contains(tile))
                {

                }
                else if (openList.Contains(tile))
                {
                    float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

                    if (tempG < tile.g)
                    {
                        tile.parent = t;
                        tile.g = tempG;
                        tile.f = tile.g + tile.h;
                    }
                }
                else
                {
                    tile.parent = t;
                    tile.g = t.g = Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, targetF.transform.position);
                    tile.f = tile.g + tile.h;

                    openList.Add(tile);
                }
            }
        }
    }

    public void BeginTurn()
    {
		distanceMoved = 0;
        turn = true;
    }

    public void EndTurn()
    {
		RemoveSelectableTiles();
		moving = false;
		TilesChecked = false;
        turn = false;
    }
}
