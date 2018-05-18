using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    public bool walkable = true;
    public bool current = false;
    public bool selectable = false;
    public bool target = false;
    public bool path = false;
    public bool visited = false;

    public int distance = 0;
    public Tiles parent = null;

    public List<Tiles> adjacencyList = new List<Tiles>();

    Vector3 halfExtents = new Vector3(0.25f, 0.5f, 0.25f);

    public float f = 0;
    public float g = 0;
    public float h = 0;

    void Update()
    {
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.magenta;
        }
        else if (path)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

	public void CheckNeighbours(GameObject checker, float halfHeight, Tiles targetC, int ResetAll)
    { 
		Reset(ResetAll);
        CheckTile(Vector3.forward, checker, halfHeight, targetC);
        CheckTile(-Vector3.forward, checker, halfHeight, targetC);
        CheckTile(Vector3.left, checker, halfHeight, targetC);
        CheckTile(-Vector3.left, checker, halfHeight, targetC);
    }

    public void CheckTile(Vector3 dir, GameObject checker, float halfHeight, Tiles targetC)
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position + dir, halfExtents);
        foreach (Collider item in hitColliders)
        {
            Tiles tile = item.GetComponent<Tiles>();

            if (tile != null && tile.walkable)
            {
                float diference = (tile.transform.position.y + tile.transform.localScale.y / 2 - transform.position.y - transform.localScale.y / 2);
                if (diference <= 1 && diference >= -2)
                {
                    RaycastHit hit;
                    Physics.Raycast(tile.transform.position, Vector3.up, out hit, halfHeight * 2);
					if (hit.collider == null || tile == targetC)
                    {
                        adjacencyList.Add(tile);
                    }
                }
            }
        }
    }

	public void Reset(int mode)
    {
		switch (mode){
		case 0:
			distance = 0;
			adjacencyList.Clear ();
			visited = false;
			parent = null;
			break;

		case 1:
			path = false;
			adjacencyList.Clear ();
			current = false;
			target = false;
			selectable = false;
			visited = false;
			parent = null;
			distance = 0;
			f = g = h = 0;
			break;

		case 2:
			adjacencyList.Clear ();
			current = false;
			target = false;
			selectable = false;
			visited = false;
			parent = null;
			distance = 0;
			f = g = h = 0;
			break;
		}
    }
}
