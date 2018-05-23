using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [Header("Basic Stats")]
    public new string name = "Error";
    public int hp = 5;
    int currentHp = 5;
    public int movement = 4;

    [Space(10)]

    [Header ("Primary Attributes")]
    public int Str = 1;
    public int Int = 1;
    public int Agi = 1;
    public int Des = 1;

    [Header("Secondary Attributes")]
    public int baseDamage = 1;
    public int armourPierce = 0;
    public int critModifier = 2;

    public int block = 0;
    int currentBlock = 0;
    public int blockRegen = 0;

    public int dodge = 0;
    int currentDodge = 0;

    public int armour = 0;

    public int takeDamage(int damage, int armourPierce, bool canBeDodged, bool canBeBlocked, bool crit)
    {
        // Returns the ammount of damage taken

        //On Attack

        if (canBeDodged && currentDodge > 0)
        {
            dodgeAttack();
            return 0;
        }

        // On Hit

        if (canBeBlocked && currentBlock > 0)
        {
            damage -= blockAttack(damage);
        }

        if (damage > 0)
        {
            // On Connect

            if (armour > armourPierce)
            {
                damage -= armour - armourPierce;
                // On Armour
            }

            if (damage > 0)
            {
                // On Damage
                if (crit)
                {
                    critical(damage);
                }
            }

            hp -= damage;

            if (hp <= 0)
                death();

            return damage;
        }

        return 0;
    }

    int blockAttack(int damage)
    {
        // Returns the ammount of damage blocked
        int damageBlocked = 0;

        if (currentBlock > damage)
        {
            damageBlocked = damage;
            currentBlock -= damage;
        }
        else
        {
            damageBlocked = currentBlock;
            currentBlock = 0;
        }

        // On Block

        return damageBlocked;
    }

    void dodgeAttack()
    {
        currentDodge--;

        // On Dodge
    }

    int critical(int damage)
    {
        damage *= critModifier;

        // On Crit

        return damage;
    }

    void death()
    {

    }
}