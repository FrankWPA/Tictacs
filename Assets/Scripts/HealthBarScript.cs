using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {

    public Character character;
    public Slider healthSlider;
    public Slider blockSlider;
    public Image barBorder;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        healthSlider.maxValue = character.hp;
        healthSlider.value = character.currentHp;

        transform.LookAt(Camera.main.transform);

        blockSlider.maxValue = character.block;
        blockSlider.value = character.currentBlock;

        if (!character.GetActionState(Actions.Attack)) barBorder.color = Color.yellow;
        else barBorder.color = Color.black;
    }
}
