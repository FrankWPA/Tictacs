using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;      

public class Character : MonoBehaviour {

	// Lists
	private List<List<object[]>> TriggerList = new List<List<object[]>>();
    public List<StatusEffect> statusEffect = new List<StatusEffect>();

	public Damage newDamage;

    public Character DamageTarget; 

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
	public int baseDamage = 2;
	public int critModifier = 2;
	public int armourPierce = 1;

	public int armour = 2;

	//public int block = 5;
	public int currentBlock = 6;
	public int blockRegen = 0;

	//public int dodge = 0;
	public int currentDodge = 1;

	public void Start()
	{
		// Adding Triggers
		AddToList ("atk_blockTrigger", new object[] { "Critical" });
        //AddToList("atk_critTrigger", new object[] { "Poison", 2});
    }

	private void AddToList (string triggerList, object[] trigger){
		foreach (List<object[]> tL in TriggerList) {
			if ((string) tL [0] [0] == triggerList) {
				tL.Add (trigger);
				return;
			}
		}
        TriggerList.Add (new List<object[]> { new object[] {triggerList}});
		AddToList (triggerList, trigger);
	}

	public void Update(){
        if (this.GetComponent<TacticsMove>().turn == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CauseDamage(DamageTarget);
            }
        }
       
	}

	public void CauseDamage(Character character)
	{
		newDamage = new Damage
		{
			actor = this,
            target = character,
			damage = this.baseDamage,
			critModifier = this.critModifier,
			armourPierce = this.armourPierce,
			canBeDodged = true,
			canBeBlocked = true,
			TriggerList = this.TriggerList,
		};

        Debug.Log("Total damage: " + character.TakeDamage (newDamage).damage);
        //character.TakeDamage(newDamage);
    }

	public Damage TakeDamage(Damage receivedDamage)
	{
		newDamage = receivedDamage;
        
        //On Resolving an Hit - attackTrigger
        callCombatEvents("attackTrigger");

		if (newDamage.canBeDodged && currentDodge > 0)
		{
            // On Dodging a hit- dodgeTrigger
            callCombatEvents("dodgeTrigger");

            currentDodge--;
            newDamage.damage = 0;
			return newDamage;
		}

        // On resolving an undodged Hit - hitTrigger
        callCombatEvents("hitTrigger");

		if (newDamage.canBeBlocked && currentBlock > 0)
		{
            // On Blocking damage - blockTrigger
            callCombatEvents("blockTrigger");

            int blockedDamage = Mathf.Min(currentBlock, newDamage.damage);
            currentBlock -= blockedDamage;
            newDamage.damage -= blockedDamage;
            
        }

		if (newDamage.damage > 0)
		{
            // On Resolving unblocked damage - connectTrigger
            callCombatEvents("connectTrigger");

			if (armour > newDamage.armourPierce)
			{
                // On Damage reduced by Armour - armourTrigger
                callCombatEvents("armourTrigger");

                newDamage.damage -= armour - newDamage.armourPierce;
            }

			if (newDamage.damage > 0)
			{
                // On Dealing/Taking Damage - damageTrigger
                callCombatEvents("damageTrigger");
			}

			currentHp -= Mathf.Max(newDamage.damage, 0);

			if (currentHp <= 0) {
                // On Killing/Dying - DeathTrigger
                callCombatEvents("deathTrigger");
			}
            return newDamage;
		}

		newDamage.damage = 0;

		return newDamage;
	}

	void callCombatEvents(string eventName)
    {
        EventTrigger(newDamage.actor, newDamage.TriggerList, ("atk_" + eventName));
        EventTrigger(newDamage.target, newDamage.target.TriggerList, ("def_" + eventName));
    }

	void EventTrigger(Character target, List<List<object[]>> eventListList, string eventName)
	{
		foreach (List<object[]> eventList in eventListList)
		{
			if ((string)eventList[0][0] == eventName) {
				for (int a = 1; a <= (eventList.Count - 1); a++){
					object[] arguments = new object[eventList[a].Length - 1];
					for (int i = 1; i < eventList[a].Length; i++) {
						arguments [i - 1] = eventList[a] [i];
					}
					InvokeStringMethod (target ,(string)eventList[a][0], arguments);
				}

			}
		}
	}

	public void InvokeStringMethod (Character target, string methodName, object[] args)
	{
		MethodInfo methodInfo = typeof(PlayerCharacter).GetMethod (methodName);
		methodInfo.Invoke(target, args);
	}

    public object VarParser(object variable)
    {
        switch ((string)variable)
        {
            case "hp": return newDamage.damage;
            default: return variable;
        }
    }

    // ---------------------- Triggered Events -----------------------------

    public void Critical()
    {  
        if (!newDamage.hasCrited)
        {
            Debug.Log("Critical Attack!");
            newDamage.damage *= newDamage.critModifier;
            newDamage.hasCrited = true;

            // On Crit - critTrigger
            callCombatEvents("critTrigger");
        }
    }

    public void Poison(object stack)
    {
        Debug.Log("Poison!");

        // On poison - poisonTrigger
        callCombatEvents("poisonTrigger");
        
        for (int i = 0; i < (int)stack; i++)
        {
            newDamage.target.statusEffect.Add(new StatusEffect { type = "poison", damage = 1 });
        }
    }
}