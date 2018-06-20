using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject cameraBase;
    public Camera cam;

    public bool follow = false;
    public bool camFollow = false;
    public Vector3 newCameraPos;

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cameraBase = GameObject.FindGameObjectWithTag("CameraBase");
    }

    void Update()
    {
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
                        TurnManager.CurrentSelected.EndTurn();
                        hit.collider.GetComponent<TacticsMove>().BeginTurn();
                        TacticsMove currentTm = hit.collider.GetComponent<TacticsMove>();
                        TurnManager.CurrentSelected = currentTm;
                        currentTm.MoveAction();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CraftManager.UpdateRecipes();
            GetComponent<Recipe>().Craft();
        }

        if (TurnManager.UnitList.Count > 0 && !TurnManager.combatInitialized) { 
            TurnManager.InitCombat();
            follow = true;
        }

        if (camFollow)
        {
            newCameraPos = new Vector3(newCameraPos.x, cameraBase.transform.position.y, newCameraPos.z);
            cameraBase.transform.position = Vector3.MoveTowards(cameraBase.transform.position, newCameraPos, cameraBase.GetComponent<CameraControl>().panSpeed);
            if (cameraBase.transform.position == newCameraPos)
            {
                camFollow = false;
            }
        }

        if (follow)
        {
            transform.position = TurnManager.CurrentSelected.transform.position + new Vector3(0, 2, 0);
        }
    }

    public void Attack()
    {
        Character currentChar = TurnManager.CurrentSelected.GetComponent<Character>();
        TacticsMove currentTm = TurnManager.CurrentSelected.GetComponent<TacticsMove>();
        currentChar.CauseDamage(currentChar.damageTarget);

        currentTm.charChar.ActionUse[currentTm.charChar.ActionCost[Actions.Attack]] = true;
    }

    public void EndTurn()
    {
        TurnManager.EndTurn();
    }
}
