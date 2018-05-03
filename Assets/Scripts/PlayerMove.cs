﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove {
	bool makingPath = false;
	int layerMask;
	bool TilesChecked = false;

	void Start () {
		Init ();
		layerMask = 1 << 8;
		//layerMask = ~layerMask;
	}
	
	void Update () {
		if (!turn){
			return;
		}
		if (currentMoveType == MoveType.Line) {
			if (!moving) {
				if (!TilesChecked) {
					FindSelectableTiles ();
					TilesChecked = true;
				}
				MakePath ();
			} 
			else {
				TilesChecked = false;
				MoveLine ();
			}
		} 
		else if (currentMoveType == MoveType.Selection) {
			if (!moving) {
				if (!TilesChecked) {
					FindSelectableTiles ();
					TilesChecked = true;
				}
				CheckMouse ();
			} 
			else {
				TilesChecked = false;
				MoveSelection ();
			}
		}
	}

	void CheckMouse(){
		if (Input.GetMouseButtonUp(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
				if (hit.collider.tag == "Tile"){
					Tiles t = hit.collider.GetComponent<Tiles>();
					if (t.selectable){
						MoveToTile(t);
					}
				}
			}
		}
	}

	void MakePath(){
		if ((Input.GetMouseButtonUp (0) && pathList.Count < 1) || makingPath) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
				if (hit.collider.tag == "Tile") {
					if (!currentTile.path) {
						currentTile.path = true;
						pathList.Add (currentTile);
					}
					Tiles t = hit.collider.GetComponent<Tiles> ();
					Vector3 dist = (pathList [pathList.Count - 1].transform.position - t.transform.position);

					if (pathList.Count < (moveDistance + 1) && t.selectable && !t.path && !t.current && (t.adjacencyList.Contains (pathList [pathList.Count - 1]))) { 
						t.path = true;
						pathList.Add (t);
					} 
					else if (t.path) {
						int pos = pathList.IndexOf (t);
						while (pathList.Count - 1 > pos) {
							pathList [pathList.Count - 1].path = false;
							pathList.RemoveAt (pathList.Count - 1);
						}
					} 
					else if ((dist.x == 0 && dist.z > 1|| dist.z == 0 && dist.x > 1) && t.selectable && dist.y <= 1) {
						Vector3 lastPath = pathList [pathList.Count - 1].transform.position;
						dist.y = 0;
						bool stop = false;
						for (int i = 1; i <= Mathf.Abs (dist.x != 0 ? dist.x : dist.z); i++) {
							if (stop == true)
								break;
							RaycastHit scanner;
							Physics.Raycast (lastPath + new Vector3 (0, -moveSpeed, 0) - (dist.normalized * i), Vector3.up, out scanner, Mathf.Infinity, layerMask);
							if (scanner.collider != null) {
								Tiles scannedTile = scanner.collider.GetComponent<Tiles> ();
								if (scannedTile.selectable) {
									if (scannedTile.path) {
										while (pathList.Count - 1 > pathList.IndexOf (scannedTile)) {
											pathList [pathList.Count - 1].path = false;
											pathList.RemoveAt (pathList.Count - 1);
										}
									} else if (scannedTile.selectable) {
										scannedTile.path = true;
										pathList.Add (scannedTile);
									}
								} else {
									stop = true;
								}
							} else {
								stop = true;
							}
						}
						while (pathList.Count - 1 >  moveDistance) {
							pathList [pathList.Count - 1].path = false;
							pathList.RemoveAt (pathList.Count - 1);
						}
							
					}
				}
				if (pathList.Count >= 1) {
					makingPath = true;
				}
				if (Input.GetMouseButtonDown (0)) {
					makingPath = false;
				}
			}
			if (!makingPath && pathList.Count > 1) {
				MoveToTile (pathList);
			}
		}
	}
}
