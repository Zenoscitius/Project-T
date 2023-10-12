using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public Inventory itemInventory;
    public int speed;
    public int maxHealth;
    public int currentHealth;
    public HealthBar healthBar;
    public ScriptableWeapon activeWeapon;
    public int attack;
    public int attackRange;
    public int might, dexterity, avo;
    public double hit, crit;
    public GameObject exhaustHighlight;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        SwitchWeapon(itemInventory.FindFirstWeaponInInventory());
        avo = 50;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void SwitchWeapon(ScriptableWeapon weapon)
    {
        if (weapon != null) activeWeapon = weapon;
        attack = activeWeapon.attack + might;
        hit = dexterity * activeWeapon.speed;
        crit = dexterity * 0.01;
        attackRange = activeWeapon.attackRange;
    }
}
