using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;      

public class Character : MonoBehaviour {

	// Lists
	private List<List<object[]>> atk_TriggerList = new List<List<object[]>>();
	public List<StatusEffect> statusEffect = new List<StatusEffect>();

	private Damage lastDamage;

	private string[] allTriggers = {
		"atk_onAttackTriggers",
		"atk_onHitTriggers",
		"atk_onConnectTriggers",
		"atk_onBlockTriggers",
		"atk_onArmourTriggers",
		"atk_onDamageTriggers",
		"atk_onCritTriggers",
		"atk_onDodgeTriggers"
	};

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
		// Adding Triggers
		AddToList (atk_TriggerList, "atk_onBlockTriggers", new object[] {"Critical"});
		AddToList (atk_TriggerList, "atk_onCritTriggers", new object[] {"Poison", 2});
	}

	private void AddToList (List<List<object[]>> list, string triggerList, object[] trigger){
		foreach (List<object[]> tL in list) {
			if ((string) tL [0] [0] == triggerList) {
				tL.Add (trigger);
				return;
			}
		}
		//Debug.Log ("Creating new Trigger " + triggerList);
		list.Add (new List<object[]> { new object[] {triggerList}});
		AddToList (list, triggerList, trigger);
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

		character.TakeDamage (newDamage);
	}

	public Damage TakeDamage(Damage newDamage)
	{
		lastDamage = newDamage;
		// Returns the ammount of damage taken

		//On Attack - atk_onAttackTriggers
		EventTrigger(lastDamage.atk_TriggerList,"atk_onAttackTriggers");

		if (lastDamage.canBeDodged && currentDodge > 0)
		{
			DodgeAttack();
			lastDamage.damage = 0;
			return lastDamage;
		}

		// On Hit - atk_onHitTriggers
		EventTrigger(lastDamage.atk_TriggerList, "atk_onHitTriggers");

		if (lastDamage.canBeBlocked && currentBlock > 0)
		{
			// On Block - atk_onBlockTriggers
			EventTrigger(lastDamage.atk_TriggerList, "atk_onBlockTriggers");

			lastDamage.damage -= BlockAttack();
		}

		if (lastDamage.damage > 0)
		{
			// On Connect - atk_onConnectTriggers
			EventTrigger(lastDamage.atk_TriggerList, "atk_onConnectTriggers");

			if (armour > lastDamage.armourPierce)
			{
				lastDamage.damage -= (Mathf.Max (armour - armourPierce, 0));

				// On Armour - atk_onArmourTriggers
				EventTrigger(lastDamage.atk_TriggerList, "atk_onArmourTriggers");
			}

			if (lastDamage.damage > 0)
			{
				
				// On Damage - atk_onDamageTriggers
				EventTrigger(lastDamage.atk_TriggerList, "atk_onDamageTriggers");
			}

			currentHp -= lastDamage.damage;

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
		EventTrigger(lastDamage.atk_TriggerList, "atk_onDodgeTriggers");
	}

	void Death()
	{
		Debug.Log (name + " died. RIP in peace.");
	}

	public void Critical()
	{
		if (!lastDamage.hasCrited) {
			Debug.Log ("Critical Attack!");
			lastDamage.damage *= lastDamage.critModifier;
			lastDamage.hasCrited = true;

			// On Crit
			EventTrigger (lastDamage.atk_TriggerList, "atk_onCritTriggers");
		}
	}

	public void Poison(object stack)
	{
		Debug.Log (stack + " Poison" + ((int)stack > 1 ? "(s)" : "") +  " applied!");
		for (int i = 0; i < (int)stack; i++) {
			statusEffect.Add(new StatusEffect { type = "poison", damage = 1 });
		}
	}

	void EventTrigger(List<List<object[]>> eventListList, string eventName)
	{
		foreach (List<object[]> eventList in eventListList)
		{
			if ((string)eventList[0][0] == eventName) {
				for (int a = 1; a <= (eventList.Count - 1); a++){
					object[] arguments = new object[eventList[a].Length - 1];
					for (int i = 1; i < eventList[a].Length; i++) {
						arguments [i - 1] = eventList[a] [i];
					}
					InvokeStringMethod ((string)eventList[a][0], arguments);
				}

			}
		}
	}

	public void InvokeStringMethod (string methodName, object[] args)
	{
		MethodInfo methodInfo = typeof(PlayerCharacter).GetMethod (methodName);
		methodInfo.Invoke(this, args);
	}

	public object VarParser(string variable)
	{
		switch (variable)
		{
		//case "damage": return lastDamage.damage;
		default: return 0;
		}
	}
}