using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButton : MonoBehaviour {

    TacticsMove tacticsMove;

    Button button;

    void Start () {
        button = GetComponent<Button>();
	}
	
	void Update () {
        if (TurnManager.combatInitialized)
        {
            tacticsMove = TurnManager.CurrentSelected;

            if (!tacticsMove.ActionUse[tacticsMove.ActionCost[Actions.Attack]])
            {
                button.interactable = true;
                return;
            }
        }
        button.interactable = false;
    }
}
