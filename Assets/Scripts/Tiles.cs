using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour {
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

	void Update () {
		if (current){
			GetComponent<Renderer>().material.color = Color.magenta;
		}
		else if (path){
			GetComponent<Renderer>().material.color = Color.yellow;
		}
		else if (target){
			GetComponent<Renderer>().material.color = Color.green;
		}
		else if (selectable){
			GetComponent<Renderer>().material.color = Color.red;
		}
		else{
			GetComponent<Renderer>().material.color = Color.white;
		}
	}

	public void CheckNeighbours(GameObject checker, float  halfHeight){
		CheckTile (Vector3.forward, checker, halfHeight);
		CheckTile (-Vector3.forward, checker, halfHeight);
		CheckTile (Vector3.left, checker, halfHeight);
		CheckTile (-Vector3.left, checker, halfHeight);
	}

	public void CheckTile(Vector3 dir, GameObject checker, float  halfHeight){
		Collider[] hitColliders = Physics.OverlapBox(transform.position + dir, halfExtents);
		foreach (Collider item in hitColliders) {
			Tiles tile = item.GetComponent<Tiles>();

			if (tile != null && tile.walkable) {
				float diference = (tile.transform.position.y + tile.transform.localScale.y / 2 - transform.position.y - transform.localScale.y / 2);
				if (diference <= 1 && diference >= -2) {
					RaycastHit hit;
					Physics.Raycast (tile.transform.position, Vector3.up, out hit, halfHeight * 2);
					if (hit.collider == null) {
						adjacencyList.Add (tile);
					}
				}
			}
		}
	}

	public void Reset(){
		adjacencyList.Clear();
		current = false;
		target = false;
		selectable = false;
		path = false; 
		visited = false;
		parent = null;
		distance = 0;
	}
}
