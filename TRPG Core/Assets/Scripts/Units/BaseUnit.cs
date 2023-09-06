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

    public GameObject exhaustHighlight;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        //activeWeapon = inventory.FindFirstWeaponInInventory();
        //SwitchWeapon(activeWeapon);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void SwitchWeapon(ScriptableWeapon weapon)
    {
        if (weapon != null) activeWeapon = weapon;
        attack = activeWeapon.attack;
        attackRange = activeWeapon.attackRange;
    }



}
