using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public List<List<object[]>> atk_TriggerList = new List<List<object[]>>();

    public Character actor;

    public int damage;
    public int critModifier;
    public int armourPierce;
    public bool canBeDodged;
    public bool canBeBlocked;

	public bool hasCrited = false;
}
