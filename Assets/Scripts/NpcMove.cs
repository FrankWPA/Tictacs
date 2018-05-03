using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMove : TacticsMove {
	GameObject targetUnit;
	// Use this for initialization
	void Start () {
		Init ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!turn){
			return;
		}
		else if (currentMoveType == MoveType.Selection) {
			if (!moving) {
				//FindNearestTarget ();
				CalculatePath ();
				FindSelectableTiles ();
			} 
			else {
				MoveSelection ();
			}
		}
	}

	void CalculatePath(){
		Tiles targetTile = GetTargetTile (targetUnit);
	}
}
