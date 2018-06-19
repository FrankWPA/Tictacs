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
                        TurnManager.SelectedCharacterMove.EndTurn();
                        hit.collider.GetComponent<TacticsMove>().BeginTurn();
                        TacticsMove currentTm = hit.collider.GetComponent<TacticsMove>();
                        TurnManager.SelectedCharacterMove = currentTm;
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

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
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
            transform.position = TurnManager.SelectedCharacterMove.transform.position + new Vector3(0, 2, 0);
        }
    }

    public void Move()
    {
        TacticsMove currentTm = TurnManager.SelectedCharacterMove.GetComponent<TacticsMove>();
        currentTm.MoveAction();
    }

    public void Attack()
    {
        Character currentChar = TurnManager.SelectedCharacterMove.GetComponent<Character>();
        TacticsMove currentTm = TurnManager.SelectedCharacterMove.GetComponent<TacticsMove>();
        currentChar.CauseDamage(currentChar.damageTarget);

        currentTm.ActionUse[currentTm.ActionCost[Actions.Attack]] = true;
    }

    public void EndTurn()
    {
        TurnManager.EndTurn();
    }
}
