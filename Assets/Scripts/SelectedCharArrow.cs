using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharArrow : MonoBehaviour {

    float angle = 0;
    float floatSpeed = 0.1f;
    float floatAmplitude = 0.2f;
    float rotateSpeed = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.eulerAngles += new Vector3(0, rotateSpeed, 0);

        angle = angle + floatSpeed < 360 ? angle + floatSpeed : angle + floatSpeed - 360;

        transform.position = new Vector3(0, Mathf.Sin(angle) * floatAmplitude, 0) + transform.parent.position;

		
	}
}
