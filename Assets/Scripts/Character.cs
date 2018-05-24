using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;      

public class Character : MonoBehaviour {

	// Lists
	private List<List<object[]>> atk_TriggerList = new List<List<object[]>>();
	public List<StatusEffect> statusEffect = new List<StatusEffect>();

	private Damage lastDamage;

	private string[] atk_Triggers = {
		"atk_attackTrigger",
		"atk_hitTrigger",
		"atk_connectTrigger",
		"atk_blockTrigger",
		"atk_armourTrigger",
		"atk_damageTrigger",
		"atk_critTrigger",
		"atk_dodgeTrigger",
		"atk_killTrigger"
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
		AddToList (atk_TriggerList, "atk_blockTrigger", new object[] {"Critical"});
		//AddToList (atk_TriggerList, "atk_critTrigger", new object[] {"Poison", 2});
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

		Debug.Log ("Final Damage: " + character.TakeDamage (newDamage).damage);
	}

	public Damage TakeDamage(Damage newDamage)
	{
		lastDamage = newDamage;
		// Returns the ammount of damage taken

		//On Attack - atk_attackTrigger
		EventTrigger(lastDamage.atk_TriggerList,"atk_attackTrigger");

		if (lastDamage.canBeDodged && currentDodge > 0)
		{
			DodgeAttack();
			lastDamage.damage = 0;
			return lastDamage;
		}

		// On Hit - atk_hitTrigger
		EventTrigger(lastDamage.atk_TriggerList, "atk_hitTrigger");

		if (lastDamage.canBeBlocked && currentBlock > 0)
		{
			// On Block - atk_blockTrigger
			EventTrigger(lastDamage.atk_TriggerList, "atk_blockTrigger");

			lastDamage.damage -= BlockAttack();
		}

		if (lastDamage.damage > 0)
		{
			// On Connect - atk_connectTrigger
			EventTrigger(lastDamage.atk_TriggerList, "atk_connectTrigger");

			if (armour > lastDamage.armourPierce)
			{
				lastDamage.damage -= armour - lastDamage.armourPierce;

				// On Armour - atk_armourTrigger
				EventTrigger(lastDamage.atk_TriggerList, "atk_armourTrigger");
			}

			if (lastDamage.damage > 0)
			{
				// On Damage - atk_damageTrigger
				EventTrigger(lastDamage.atk_TriggerList, "atk_damageTrigger");
			}

			currentHp -= Mathf.Max(lastDamage.damage, 0);

			if (currentHp <= 0) {
				// On Killing - atk_killTrigger
				EventTrigger(lastDamage.atk_TriggerList, "atk_killTrigger");
				Death ();
			}

			return lastDamage;
		}

		lastDamage.damage = 0;

		return lastDamage;
	}

	int BlockAttack()
	{
		int blockedDamage = Mathf.Min (currentBlock, lastDamage.damage);

		currentBlock -= blockedDamage;

		// Returns the ammount of damage blocked
		return blockedDamage;
	}

	void DodgeAttack()
	{
		currentDodge--;

		// On Dodge - atk_dodgeTrigger
		EventTrigger(lastDamage.atk_TriggerList, "atk_dodgeTrigger");
	}

	void Death()
	{
		//Debug.Log (name + " died. RIP in peace.");
	}

	public void Critical()
	{
		if (!lastDamage.hasCrited) {
			Debug.Log ("Critical Attack!");
			lastDamage.damage *= lastDamage.critModifier;
			lastDamage.hasCrited = true;

			// On Crit - atk_critTrigger
			EventTrigger (lastDamage.atk_TriggerList, "atk_critTrigger");
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