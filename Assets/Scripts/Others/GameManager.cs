using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
	void Update () {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            TurnManager.InitCombat();
        }
    }
	
	public void EndTurn()
    {
        TurnManager.EndTurn();
    }
}
