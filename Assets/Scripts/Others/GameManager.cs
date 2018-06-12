using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
	void Update () {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            TurnManager.InitCombat();
        }

        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == TurnManager.CurrentTeam)
                {
                    if (!hit.collider.GetComponent<TacticsMove>().turn && !hit.collider.GetComponent<TacticsMove>().passedTurn)
                    {
                        TurnManager.CurrentTurn.EndTurn();
                        hit.collider.GetComponent<TacticsMove>().BeginTurn();
                        TurnManager.CurrentTurn = hit.collider.GetComponent<TacticsMove>();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CraftManager.UpdateRecipes();
            GetComponent<Recipe>().Craft();
        }
    }
	
	public void EndTurn()
    {
        TurnManager.EndTurn();
    }
}
