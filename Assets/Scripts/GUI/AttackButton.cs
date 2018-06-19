using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButton : MonoBehaviour {

    TacticsMove tacticsMove;

    Button button;


    // Use this for initialization
    void Start () {
        button = GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
        if (TurnManager.Ready)
        {
            tacticsMove = TurnManager.SelectedCharacterMove;

            if (!tacticsMove.ActionUse[tacticsMove.ActionCost[Actions.Attack]])
            {
                button.interactable = true;
                return;
            }
        }
        button.interactable = false;
    }
}
