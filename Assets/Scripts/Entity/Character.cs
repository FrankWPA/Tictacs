using System.Collections.Generic;
using static EventSystem.Trigger;
using UnityEngine;
//using System.Reflection; - Not being used anymore


public class Character : MonoBehaviour {

    public List<List<object[]>> triggerList = new List<List<object[]>>();
    public List<StatusEffect> statusEffect = new List<StatusEffect>();

    public Damage newDamage;
    public Character damageTarget;

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
	//public int blockRegen = 0;

	//public int dodge = 0;
	public int currentDodge = 1;

	public void Start()
	{
        // Adding Triggers
        // CreateTrigger ("Condition", new object[] { "FunctionToBeCalled", argument0, argument1, ... argumentN});
        // The function doesn't need to exist, it still can trigger other triggers as a condition.

        CreateTrigger ("atk_blockTrigger", new object[] { "Critical" });
        CreateTrigger ("atk_critTrigger", new object[] { "Poison", 2 });
    }

	public void Update(){
        if (this.GetComponent<TacticsMove>().turn == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CauseDamage(damageTarget);
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
			triggerList = this.triggerList,
		};

        //Debug.Log("Total damage: " + character.TakeDamage (newDamage).damage);
        character.TakeDamage(newDamage);
    }

	public Damage TakeDamage(Damage receivedDamage)
	{
		newDamage = receivedDamage;
        
        //On Resolving an Hit - attackTrigger
        CallCombatEvents("attackTrigger");

		if (newDamage.canBeDodged && currentDodge > 0)
		{
            // On Dodging a hit- dodgeTrigger
            CallCombatEvents("dodgeTrigger");

            currentDodge--;
            newDamage.damage = 0;
			return newDamage;
		}

        // On resolving an undodged Hit - hitTrigger
        CallCombatEvents("hitTrigger");

		if (newDamage.canBeBlocked && currentBlock > 0)
		{
            // On Blocking damage - blockTrigger
            CallCombatEvents("blockTrigger");

            int blockedDamage = Mathf.Min(currentBlock, newDamage.damage);
            currentBlock -= blockedDamage;
            newDamage.damage -= blockedDamage;
            
        }

		if (newDamage.damage > 0)
		{
            // On Resolving unblocked damage - connectTrigger
            CallCombatEvents("connectTrigger");

			if (armour > newDamage.armourPierce)
			{
                // On Damage reduced by Armour - armourTrigger
                CallCombatEvents("armourTrigger");

                newDamage.damage -= armour - newDamage.armourPierce;
            }

			if (newDamage.damage > 0)
			{
                // On Dealing/Taking Damage - damageTrigger
                CallCombatEvents("damageTrigger");
			}

			currentHp -= Mathf.Max(newDamage.damage, 0);

			if (currentHp <= 0) {
                // On Killing/Dying - DeathTrigger
                CallCombatEvents("deathTrigger");
			}
            return newDamage;
		}

		newDamage.damage = 0;

		return newDamage;
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
            CallCombatEvents("critTrigger");
        }
    }

    public void Poison(object stack)
    {
        Debug.Log("Poison!");

        // On poison - poisonTrigger
        CallCombatEvents("poisonTrigger");
        
        for (int i = 0; i < (int)stack; i++)
        {
            newDamage.target.statusEffect.Add(new StatusEffect { type = "poison", damage = 1 });
        }
    }
}