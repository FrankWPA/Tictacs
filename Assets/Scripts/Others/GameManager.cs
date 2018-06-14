using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject cameraBase;
    public Camera cam;

    public bool follow = false;
    public bool camFollow = false;
    public Vector3 j;

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

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            TurnManager.InitCombat();
            follow = true;
        }

        if (camFollow)
        {
            j = new Vector3(j.x, cameraBase.transform.position.y, j.z);
            cameraBase.transform.position = Vector3.MoveTowards(cameraBase.transform.position, j, cameraBase.GetComponent<CameraControl>().panSpeed);
            if (cameraBase.transform.position == j)
            {
                camFollow = false;
            }
        }

        if (follow)
        {
            transform.position = TurnManager.CurrentTurn.transform.position + new Vector3(0, 2, 0);
        }
    }

    public void Move()
    {
        TacticsMove currentTm = TurnManager.CurrentTurn.GetComponent<TacticsMove>();
        currentTm.MoveAction();
    }

    public void Attack()
    {
        Character currentChar = TurnManager.CurrentTurn.GetComponent<Character>();
        TacticsMove currentTm = TurnManager.CurrentTurn.GetComponent<TacticsMove>();
        currentChar.CauseDamage(currentChar.damageTarget);

        currentTm.b[currentTm.a[TacticsMove.Actions.Attack]] = true;
    }

    public void EndTurn()
    {
        TurnManager.EndTurn();
    }
}
