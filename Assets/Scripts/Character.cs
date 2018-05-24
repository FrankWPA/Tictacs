using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Character : MonoBehaviour {

	// Event Triggers
	private List<object[]> atk_onAttackTriggers = new List<object[]>();
	private List<object[]> atk_onHitTriggers = new List<object[]>();
	private List<object[]> atk_onConnectTriggers = new List<object[]>();
	private List<object[]> atk_onBlockTriggers = new List<object[]>();
	private List<object[]> atk_onArmorTriggers = new List<object[]>();
	private List<object[]> atk_onDamageTriggers = new List<object[]>();

	private List<object[]> atk_onCritTriggers = new List<object[]>();
	private List<object[]> atk_onDodgeTriggers = new List<object[]>();

	// Lists
	private List<List<object[]>> atk_TriggerList = new List<List<object[]>>();
	public List<StatusEffect> statusEffect = new List<StatusEffect>();

	private Damage lastDamage;

	[Header("Basic Stats")]
	public new string name = "Error";
	public int hp = 5;
	public int currentHp = 5;
	public int movement = 4;

	[Space(10)]

	[Header ("Primary Attributes")]
	public int Str = 1;
	public int Int = 1;
	public int Agi = 1;
	public int Des = 1;

	[Header("Secondary Attributes")]
	public int baseDamage = 3;
	public int critModifier = 3;
	public int armourPierce = 0;

	public int armour = 0;

	//public int block = 5;
	public int currentBlock = 5;
	public int blockRegen = 0;

	//public int dodge = 0;
	public int currentDodge = 0;


	public void Start()
	{
		// Preparation
		atk_onAttackTriggers.Add(new object[] {"atk_onAttackTriggers"});;
		atk_onHitTriggers.Add(new object[] {"atk_onHitTriggers"});
		atk_onConnectTriggers.Add(new object[] {"atk_onConnectTriggers"});
		atk_onBlockTriggers.Add(new object[] {"atk_onBlockTriggers"});
		atk_onArmorTriggers.Add(new object[] {"atk_onArmorTriggers"});
		atk_onDamageTriggers.Add(new object[] {"atk_onDamageTriggers"});
		atk_onCritTriggers.Add(new object[] {"atk_onCritTriggers"});
		atk_onDodgeTriggers.Add (new object[] {"atk_onDodgeTriggers"});

		// Adding Triggers
		atk_onBlockTriggers.Add(new object[] {"Critical"});
		//atk_onCritTriggers.Add(new object[] {"Poison", 2});

		// Updating List of Triggers
		atk_TriggerList.Add(atk_onAttackTriggers);
		atk_TriggerList.Add(atk_onHitTriggers);
		atk_TriggerList.Add(atk_onConnectTriggers);
		atk_TriggerList.Add(atk_onBlockTriggers);
		atk_TriggerList.Add(atk_onArmorTriggers);
		atk_TriggerList.Add(atk_onDamageTriggers);
		atk_TriggerList.Add(atk_onCritTriggers);
		atk_TriggerList.Add(atk_onDodgeTriggers);
	}

	public void Update(){
		if (Input.GetKeyDown (KeyCode.Space)){
			CauseDamage (this);
		}
	}

	public void CauseDamage(Character character)
	{
		Damage newDamage = new Damage
		{
			actor = this,
			damage = this.baseDamage,
			critModifier = this.critModifier,
			armourPierce = this.armourPierce,
			canBeDodged = true,
			canBeBlocked = true,
			atk_TriggerList = this.atk_TriggerList,
		};

		Debug.Log("Final Damage: " + character.TakeDamage(newDamage).damage);
	}

	public Damage TakeDamage(Damage newDamage)
	{
		lastDamage = newDamage;
		// Returns the ammount of damage taken

		//On Attack - atk_onAttackTriggers
		EventTriggers(lastDamage.atk_TriggerList,"atk_onAttackTriggers");

		if (lastDamage.canBeDodged && currentDodge > 0)
		{
			DodgeAttack();
			lastDamage.damage = 0;
			return lastDamage;
		}

		// On Hit - atk_onHitTriggers
		EventTriggers(lastDamage.atk_TriggerList, "atk_onHitTriggers");

		if (lastDamage.canBeBlocked && currentBlock > 0)
		{
			// On Block - atk_onBlockTriggers
			EventTriggers(lastDamage.atk_TriggerList, "atk_onBlockTriggers");

			lastDamage.damage -= BlockAttack();
		}

		if (lastDamage.damage > 0)
		{
			// On Connect - atk_onConnectTriggers
			EventTriggers(lastDamage.atk_TriggerList, "atk_onConnectTriggers");

			if (armour > lastDamage.armourPierce)
			{
				lastDamage.damage -= (armour - armourPierce);
				if (lastDamage.damage < 0)
					lastDamage.damage = 0;
				// On Armour - atk_onArmorTriggers
				EventTriggers(lastDamage.atk_TriggerList, "atk_onArmorTriggers");
			}

			if (lastDamage.damage > 0)
			{
				
				// On Damage - atk_onDamageTriggers
				EventTriggers(lastDamage.atk_TriggerList, "atk_onDamageTriggers");
			}

			hp -= lastDamage.damage;

			if (hp <= 0)
				Death();

			return lastDamage;
		}

		lastDamage.damage = 0;

		return lastDamage;
	}

	int BlockAttack()
	{

		int damageBlocked = 0;

		if (currentBlock > lastDamage.damage)
		{
			damageBlocked = lastDamage.damage;
			currentBlock -= lastDamage.damage;
		}
		else
		{
			damageBlocked = currentBlock;
			currentBlock = 0;
		}

		// Returns the ammount of damage blocked
		return damageBlocked;
	}

	void DodgeAttack()
	{
		currentDodge--;

		// On Dodge - atk_onDodgeTriggers
		EventTriggers(lastDamage.atk_TriggerList, "atk_onDodgeTriggers");
	}

	public void Critical()
	{
		if (!lastDamage.hasCrited) {
			Debug.Log ("Critical Attack!");
			lastDamage.damage *= lastDamage.critModifier;
			lastDamage.hasCrited = true;

			// On Crit
			EventTriggers (lastDamage.atk_TriggerList, "atk_onCritTriggers");
		}
	}

	public void Poison(object stack)
	{
		Debug.Log (stack + " Poison" + ((int)stack > 1 ? "(s)" : "") +  " applied!");
		for (int i = 0; i < (int)stack; i++) {
			statusEffect.Add(new StatusEffect { type = "poison", damage = 1 });
		}
	}

	void Death()
	{
		Debug.Log (name + " died. RIP in peace.");
	}

	void EventTriggers(List<List<object[]>> eventListList, string eventName)
	{
		foreach (List<object[]> eventList in eventListList)
		{
			if ((string)eventList[0][0] == eventName) {
				for (int a = 1; a <= (eventList.Count - 1); a++){
					object[] arguments = new object[eventList[a].Length - 1];
					for (int i = 1; i < eventList[a].Length; i++) {
						arguments [i - 1] = eventList[a] [i];
					}

					//Debug.Log ("Trying to invoke " + (string)eventList[a][0]);
					InvokeStringMethod2 ((string)eventList[a][0], arguments);
				}

			}
		}
	}

	public void InvokeStringMethod2 (string methodName, object[] args)
	{
		MethodInfo methodInfo = typeof(PlayerCharacter).GetMethod (methodName);
		methodInfo.Invoke(this, args);
	}

	public int VarParser(string variable)
	{
		switch (variable)
		{
		//case "damage": return lastDamage.damage;
		default: return 0;
		}
	}
}