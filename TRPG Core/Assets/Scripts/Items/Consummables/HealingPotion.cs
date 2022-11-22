using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotion : ScriptableConsummable
{
    new public void UseItem(BaseUnit activeUnit)
    {
        activeUnit.currentHealth += 10;
        if (activeUnit.currentHealth > activeUnit.maxHealth) activeUnit.currentHealth = activeUnit.maxHealth;
    }
}
