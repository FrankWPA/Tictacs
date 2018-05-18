using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character {

    [Header("Presets")]
    public job classToApply = job.noone;

    [ContextMenu("Apply Class")]
    public void applyPreset()
    {
        switch (classToApply)
        {
            /* ---For new classes---
             * 
             * - 10 points in total
             * - Each Destiny costs 2 points
             */

            // 1 High Stat (5, 2, 1)
            case job.barbarian: Str = 5; Int = 1; Agi = 2; Des = 1; break;
            case job.wizard: Str = 1; Int = 5; Agi = 2; Des = 1; break;
            case job.rogue: Str = 1; Int = 2; Agi = 5; Des = 1; break;
            case job.knight: Str = 5; Int = 2; Agi = 1; Des = 1; break;
            case job.archer: Str = 2; Int = 1; Agi = 5; Des = 1; break;
            case job.druid: Str = 2; Int = 5; Agi = 1; Des = 1; break;

            // 2 Medium Stats (4, 3, 1)
            case job.cleric: Str = 3; Int = 4; Agi = 1; Des = 1; break;
            case job.paladin: Str = 4; Int = 3; Agi = 1; Des = 1; break;
            case job.mercenary: Str = 4; Int = 1; Agi = 3; Des = 1; break;
            case job.highwayman: Str = 3; Int = 1; Agi = 4; Des = 1; break;
            case job.kleptomancer: Str = 1; Int = 4; Agi = 3; Des = 1; break;
            case job.ninja: Str = 1; Int = 3; Agi = 4; Des = 1; break;

            // Balanced Stats (3, 3, 2)
            case job.swashbuckler: Str = 3; Int = 2; Agi = 3; Des = 1; break;
            case job.noble: Str = 2; Int = 3; Agi = 3; Des = 1; break;
            case job.warlord: Str = 3; Int = 3; Agi = 2; Des = 1; break;

            // Specialists (6, 1, 1)
            case job.acrobat: Str = 1; Int = 1; Agi = 6; Des = 1; break;
            case job.berserker: Str = 1; Int = 1; Agi = 6; Des = 1; break;
            case job.scholar: Str = 1; Int = 1; Agi = 6; Des = 1; break;

            // 2 Destiny
            case job.adventurer: Str = 2; Int = 2; Agi = 2; Des = 2; break;
            case job.chosenOne: Str = 3; Int = 2; Agi = 1; Des = 2; break;
            case job.hermit: Str = 1; Int = 4; Agi = 1; Des = 2; break;
            case job.luckyBastard: Str = 1; Int = 1; Agi = 4; Des = 2; break;
            case job.doctorDestiny: Str = 0; Int = 0; Agi = 0; Des = 5; break;

            default: Str = 1; Int = 1; Agi = 1; Des = 1; break;
        }
    }
}

// List of Classes
public enum job
{
    noone, barbarian, wizard, rogue, adventurer, cleric, swashbuckler, chosenOne, paladin, mercenary,
    archer, ninja, kleptomancer, knight, highwayman, druid, noble, warlord, acrobat, berserker, scholar,
    hermit, doctorDestiny, luckyBastard
}