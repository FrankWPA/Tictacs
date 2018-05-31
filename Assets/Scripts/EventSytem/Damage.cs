using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public List<List<object[]>> triggerList = new List<List<object[]>>();

    public Character actor;
    public Character target;

    public int damage;
    public int critModifier;
    public int armourPierce;
    public bool canBeDodged;
    public bool canBeBlocked;

	public bool hasCrited = false;
}