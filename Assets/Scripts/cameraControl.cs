using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour {

    [Range(0.1f, 10f)]
    public float panSpeed = 1f;

    [Range(0.1f, 10f)]
    public float rotateSpeed = 1f;

    [Range(0.1f, 10f)]
    public float zoomSpeed = 1f;

    public float minZoom = 10f;
    public float maxZoom = 60f;

    float panDist = 10f;
    float panSpeedModifier = 0.5f;

    Camera cam;

    // Use this for initialization
    void Start () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {

        panCamera();
        rotateCamera();
        zoomCamera();
    }

    void panCamera()
    {
        Vector2 inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.mousePosition.x < panDist) inputDir.x -= panSpeed * panSpeedModifier;
        if (Input.mousePosition.x > Screen.width - panDist) inputDir.x += panSpeed * panSpeedModifier;
        if (Input.mousePosition.y < panDist) inputDir.y -= panSpeed * panSpeedModifier;
        if (Input.mousePosition.y > Screen.height - panDist) inputDir.y += panSpeed * panSpeedModifier;

        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = right.y = 0f;
        right.y = right.y = 0f;

        forward.Normalize();
        right.Normalize();

        transform.position += right * inputDir.x * panSpeed + forward * inputDir.y * panSpeed;
    }

    void rotateCamera()
    {
        if (!Input.GetMouseButton(2))
        {
            if (Input.GetMouseButtonUp(2))
                Cursor.lockState = CursorLockMode.None;

            return;
        }

        if (Input.GetMouseButtonDown(2))
            Cursor.lockState = CursorLockMode.Locked;

        transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * rotateSpeed, 0f));
    }

    void zoomCamera()
    {
        cam.fieldOfView += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * 10f;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
    }
}
