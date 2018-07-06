using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Character : MonoBehaviour
{
    // Sisteam de ações

    public Dictionary<Actions, TurnActions> ActionCost = new Dictionary<Actions, TurnActions>() {
        {Actions.Move,  TurnActions.Movement},
        {Actions.Attack,  TurnActions.Default}
    };

    public Dictionary<TurnActions, bool> ActionUse = new Dictionary<TurnActions, bool>() {
        {TurnActions.Movement, false},
        {TurnActions.Default, false}
    };
    
    // Sisteam de ações

    public Dictionary<string, List<object[]>> triggerList = new Dictionary<string, List<object[]>>();

    public List<StatusEffect> StatusEffectList = new List<StatusEffect>();
    
    public Damage newDamage;
    Character other;
    public TacticsMove charMove;
    
    [Header("Basic Stats")]
    public new string name = "Error";
    public int hp = 5;
    public int currentHp = 5;
    public int movement = 4;

    [Space(10)]

    [Header("Primary Attributes")]
    public int Str = 1;
    public int Int = 1;
    public int Agi = 1;
    public int Des = 1;

    [Header("Secondary Attributes")]
    public int baseDamage = 1;
    public int critModifier = 2;
    public int armourPierce = 0;

    public int armour = 1;

    public int block = 1;
    public int currentBlock = 1;
    public int blockRegen = 0;

    public int dodge = 1;
    public int currentDodge = 1;

    public void Start()
    {
        charMove = GetComponent<TacticsMove>();

        // Adding Triggers
        // this.CreateTrigger("Condition", new object[] { "FunctionToBeCalled", argument0, argument1, ... argumentN});
        // The function doesn't need to exist, it still can trigger other triggers as a condition.

        //this.CreateTrigger("atk_dodgeTrigger", new object[] { "Critical"});
        //this.CreateTrigger("atk_blockTrigger", new object[] { "Critical", 1});

        //this.CreateTrigger("atk_dodgeTrigger", new object[] { "ApplyStatus", "pTarget", EffectType.Debuff, 2, new object[] { "def_damageTrigger", "Critical" } });

        //this.CreateTrigger("def_attackTrigger", new object[] { "UpdateStatus" });
        this.CreateTrigger("deathTrigger", new object[] { "Die" });
        this.CreateTrigger("res_deathTrigger", new object[] { "Die" });
    }

    public void Update() {

        if (charMove.turn)
        {
            foreach (TurnActions action in ActionUse.Keys.ToArray())
            {
                if (ActionUse[action] == false)
                {
                    return;
                }
            }
            TurnManager.EndTurn();
        }
    }

    public void StartTurn()
    {
        this.EventTrigger(triggerList, "startTurnTrigger", false);

        currentBlock = Mathf.Min(block, currentBlock + blockRegen);
    }

    public void EndTurn()
    {
        this.EventTrigger(triggerList, "endTurnTrigger", false);
    }

    public void CauseDamage()
    {
        charMove.FindNearestTarget();

        if (charMove.targetUnit != null)
        {
            Vector3 a = new Vector3(1, 0, 1);
            if (Vector3.Distance(Vector3.Scale(transform.position, a), Vector3.Scale(charMove.targetUnit.transform.position, a)) <= 1)
            {
                Character character = charMove.targetUnit.GetComponent<Character>();

                newDamage = new Damage
                {
                    actor = this,
                    target = character,
                    damage = this.baseDamage,
                    critModifier = this.critModifier,
                    armourPierce = this.armourPierce,
                    canBeDodged = true,
                    canBeBlocked = true,
                    actorTriggerList = this.triggerList,
                    targetTriggerList = character.triggerList,
                };

                //Debug.Log("Total damage: " + character.TakeDamage (newDamage).damage);
                character.TakeDamage(newDamage);
                SetActionState(Actions.Attack, true);
            }
        }
    }

    public Damage TakeDamage(Damage receivedDamage)
    {
        newDamage = receivedDamage;
        other = newDamage.actor;

        //On Resolving an Hit - attackTrigger
        other.CallCombatEvents("attackTrigger");

        if (newDamage.canBeDodged && currentDodge > 0)
        {
            // On Dodging a hit- dodgeTrigger
            this.CallCombatEvents("dodgeTrigger");

            currentDodge--;
            newDamage.damage = 0;
            return newDamage;
        }

        // On resolving an undodged Hit - hitTrigger
        other.CallCombatEvents("hitTrigger");

        if (newDamage.canBeBlocked && currentBlock > 0)
        {
            // On Blocking damage - blockTrigger
            this.CallCombatEvents("blockTrigger");

            int blockedDamage = Mathf.Min(currentBlock, newDamage.damage);
            currentBlock -= blockedDamage;
            newDamage.damage -= blockedDamage;

        }

        if (newDamage.damage > 0)
        {
            // On Resolving unblocked damage - connectTrigger
            other.CallCombatEvents("connectTrigger");

            if (armour > newDamage.armourPierce)
            {
                // On Damage reduced by Armour - armourTrigger
                this.CallCombatEvents("armourTrigger");

                newDamage.damage -= armour - newDamage.armourPierce;
            }

            if (newDamage.damage > 0)
            {
                // On Dealing/Taking Damage - damageTrigger
                other.CallCombatEvents("damageTrigger");
            }

            currentHp -= Mathf.Max(newDamage.damage, 0);

            if (currentHp <= 0) {
                // On Killing/Dying - deathTrigger
                this.CallCombatEvents("deathTrigger");
            }
            return newDamage;
        }

        newDamage.damage = 0;

        return newDamage;
    }

    public bool GetActionState(Actions action)
    {
        return ActionUse[ActionCost[action]];
    }

    public void SetActionState(Actions action, bool toSet)
    {
        ActionUse[ActionCost[action]] = toSet;
    }

    // ---------------------- Triggered Events -----------------------------

    [ContextMenu("Die")]
    public void Die()
    {
        charMove.RemoveUnit();
        Destroy(gameObject);
    }

    public void Critical()
    {
        if (!newDamage.hasCrited)
        {
            Debug.Log("Critical Attack!");
            newDamage.damage *= newDamage.critModifier;
            newDamage.hasCrited = true;

            // On Crit - critTrigger
            other.CallCombatEvents("critTrigger");
        }
    }

    public void DeathTouch()
    {
        newDamage.target.Die();
    }

    public void ApplyStatus(Character statusTarget, EffectType effectType, int turnDuration, object[] toApply)
    {
        Debug.Log(effectType + " Applied!");
        StatusEffect newStatusEffect = new StatusEffect { type = effectType };
        newStatusEffect.Owner = statusTarget;
        newStatusEffect.TurnDuration = turnDuration;

        newStatusEffect.StatusTriggerList.Add(toApply);
        newStatusEffect.ApplyTrigger();

        newDamage.target.StatusEffectList.Add(newStatusEffect);
    }

    public void RemoveStatus(EffectType effectType)
    {
        for (int i = (StatusEffectList.Count - 1); i >= 0; i--)
        {
            if (StatusEffectList[i].type == effectType) {
                StatusEffectList[i].RemoveTrigger();
                StatusEffectList.Remove(StatusEffectList[i]);
            }
        }
    }

    public void UpdateStatus(){
        foreach (StatusEffect toUpdate in StatusEffectList.ToArray())
        {
            toUpdate.TurnUpdate();
        }
    }
}

public enum Actions { Move, Attack }
public enum TurnActions { Movement, Default }