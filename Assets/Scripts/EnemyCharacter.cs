using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character {

    [Header("Presets")]
    public enemy enemyPressetToApply = enemy.noone;

    [ContextMenu("Apply Enemy Preset")]
    public void applyEnemyPreset()
    {
        switch (enemyPressetToApply)
        {
            case enemy.zombie:
                name = "Zombie";
                hp = 5; movement = 3;
                Str = 1; Int = 0; Agi = 0; Des = 0;
                break;

            case enemy.goblin:
                name = "Goblin";
                hp = 3; movement = 5;
                Str = 1; Int = 0; Agi = 1; Des = 0;
                break;

            case enemy.kobold:
                name = "Kobold";
                hp = 3; movement = 5;
                Str = 0; Int = 1; Agi = 1; Des = 0;
                break;

            default: Str = 1; Int = 1; Agi = 1; Des = 1; break;
        }
    }
}


// List of Enemy Presets
public enum enemy
{
    noone, zombie, goblin, kobold
}