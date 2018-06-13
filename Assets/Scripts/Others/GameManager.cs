using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public CameraControl cameraControl;
    public Camera cam;

    public bool follow = false;
    Vector3 h;

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cameraControl = GameObject.FindGameObjectWithTag("CameraBase").GetComponent<CameraControl>();
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
                        //follow = true;
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

        /*if (follow)
        {
            h = cam.WorldToScreenPoint(TurnManager.CurrentTurn.transform.position);
            h -= new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
            h += new Vector3(Mathf.Sign(h.x) * cam.pixelWidth, Mathf.Sign(h.y) * cam.pixelHeight);
            Vector2 pos = new Vector2(h.x, h.y);
            cameraControl.panCamera(pos);
        }*/

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
