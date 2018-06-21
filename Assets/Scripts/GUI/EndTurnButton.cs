using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour {

    Button button;

    void Start () {
        button = GetComponent<Button>();
	}
	
	void Update () {
        if (TurnManager.TurnButtonActive)
        {
            button.interactable = true;
        }
        else button.interactable = false;
    }
}
