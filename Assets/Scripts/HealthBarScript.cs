using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {

    public Character character;
    public Slider slider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        slider.maxValue = character.hp;
        slider.value = character.currentHp;

        transform.LookAt(Camera.main.transform);
	}
}
