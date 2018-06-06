using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{
    public bool makingPath = false;
    int layerMask;
	public Vector3 dist;

    void Start()
    {
        Init();
		moveDistance = this.GetComponent<PlayerCharacter> ().movement;
        layerMask = 1 << 8;
        //layerMask = ~layerMask;
    }

    void Update()
    {
        if (!turn)
        {
            return;
        }
        switch (currentMoveType)
        {
            case MoveType.Line:
                if (!moving)
                {
                    if (!TilesChecked)
                    {
                        FindSelectableTiles(1);
                        TilesChecked = true;
                    }
                    MakePath();
                }
                else
                {
                    TilesChecked = false;
                    MoveLine();
                }
                break;
            case MoveType.Selection:
                if (!moving)
                {
                    if (!TilesChecked)
                    {
                        FindSelectableTiles(1);
                        TilesChecked = true;
                    }
					CheckMouse();
                }
                else
                {
                    TilesChecked = false;
                    MoveSelection();
                }
                break;
        }
    }

    void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.tag == "Tile")
                {
                    Tiles t = hit.collider.GetComponent<Tiles>();
                    if (t.selectable)
                    {
                        MoveToTile(t);
                    }
                }
            }
        }
    }

    void MakePath()
    {
		if (((Input.GetMouseButtonUp(0) && pathList.Count < 1)) || makingPath)
		{		
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.tag == "Tile")
                {
					Tiles t = hit.collider.GetComponent<Tiles>();

                    if (!currentTile.path)
                    {
                        currentTile.path = true;
                        pathList.Add(currentTile);
                    }

					dist = ((pathList[pathList.Count - 1].transform.position - t.transform.position));

					if (pathList.Count < ((moveDistance - distanceMoved) + 1) && t.selectable && !t.path && !t.current && (t.adjacencyList.Contains (pathList [pathList.Count - 1]))) {
						//t.path = true;
						pathList.Add (t);
					} 
					else if (t.path) {
						int pos = pathList.IndexOf (t);
						while (pathList.Count - 1 > pos) {
							pathList [pathList.Count - 1].path = false;
							pathList.RemoveAt (pathList.Count - 1);
						}
					} else if ((dist.x == 0 && Mathf.Abs (dist.z) > 1 || dist.z == 0 && Mathf.Abs (dist.x) > 1) && t.selectable && dist.y <= 1) {
						Vector3 lastPath = pathList [pathList.Count - 1].transform.position;
						dist.y = 0;
						bool stop = false;
						for (int i = 1; i <= Mathf.Abs (dist.x != 0 ? dist.x : dist.z); i++) {
							if (stop == true) {
								PathToTile (t);
								goto end;
							}
							RaycastHit scanner;
							Physics.Raycast (lastPath - new Vector3 (0, halfHeight*2, 0) - (dist.normalized * i), Vector3.up, out scanner, halfHeight*2, layerMask);
							if (scanner.collider != null) {
								Tiles scannedTile = scanner.collider.GetComponent<Tiles> ();
								if (scannedTile.selectable) {
									if (scannedTile.path) {
										while (pathList.Count - 1 > pathList.IndexOf (scannedTile)) {
											pathList [pathList.Count - 1].path = false;
											pathList.RemoveAt (pathList.Count - 1);
										}
									} else {
										pathList.Add (scannedTile);
									}
								} else {
									stop = true;
								}
							} else {
								stop = true;
							}
						}
						end:
						while (pathList.Count - 1 > (moveDistance - distanceMoved)) {
							pathList [pathList.Count - 1].path = false;
							pathList.RemoveAt (pathList.Count - 1);
						}

					} else if (!t.path && pathList.Count < (moveDistance - distanceMoved)) {
						PathToTile (t);
					}
                }
                if (pathList.Count >= 1)
                {
                    makingPath = true;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    makingPath = false;
                }
            }
			foreach (Tiles tile in pathList) {
				tile.path = true;
			}
            if (!makingPath && pathList.Count > 1)
            {
                MoveToTile(pathList);
            }

            if ((((Input.GetMouseButtonUp(1) && pathList.Count > 0) || (Input.GetMouseButtonUp(0) && pathList.Count <= 1)) && makingPath))
            {
                foreach (Tiles tile in pathList)
                {
                    tile.path = false;
                }
                pathList.Clear();
                makingPath = false;
            }
        }
    }

	void PathToTile(Tiles t){
		currentTile.current = false;
		currentTile = pathList [pathList.Count - 1];
		currentTile.current = true;
		FindSelectableTiles (0);
		path.Clear();
		Tiles next = t;

		while (next != null){
			path.Push(next);
			next = next.parent;
		}
		path.Pop();
		while (path.Count > 0){
			t = path.Peek();

            if (t.path)
				goto end2;
			else if (t.selectable && (t.adjacencyList.Contains(pathList[pathList.Count - 1]))) {
                pathList.Add (t);
			}
            path.Pop();
		}
		end2:
		FindSelectableTiles (2);
	}
}
