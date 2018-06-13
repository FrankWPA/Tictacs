using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    [Range(0.1f, 10f)]
    public float panSpeed = 1f;

    [Range(0.1f, 10f)]
    public float rotateSpeed = 1f;

    [Range(0.1f, 10f)]
    public float zoomSpeed = 1f;

    public float minZoom = 10f;
    public float maxZoom = 60f;

	public bool borderPan = true;
	public bool invertZoom = false;

    float panDist = 10f;
    float panSpeedModifier = 0.5f;

    Camera cam;
    
    void Start () {
        cam = Camera.main;
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) borderPan = !borderPan;
        if (borderPan)
        {
            panCamera(Input.mousePosition);
        }
        rotateCamera();
        zoomCamera();
    }

    public void panCamera(Vector2 pos)
    {
        Vector2 inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
		if (pos.x < panDist)
			inputDir.x -= panSpeed * panSpeedModifier;
		if (pos.x > Screen.width - panDist)
			inputDir.x += panSpeed * panSpeedModifier;
		if (pos.y < panDist)
			inputDir.y -= panSpeed * panSpeedModifier;
		if (pos.y > Screen.height - panDist)
			inputDir.y += panSpeed * panSpeedModifier;

        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = right.y = 0f;

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
		cam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * 10f * (invertZoom ? -1 : 1);
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
    }
}
