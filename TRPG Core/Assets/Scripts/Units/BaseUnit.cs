using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public int speed;
    public int maxHealth;
    public int currentHealth;
    public HealthBar healthBar;
    public List<BaseItem> inventory;
    public BaseWeapon activeWeapon;
    public int attack;
    public int attackRange;
    public Stats stats;
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        activeWeapon = FindFirstWeaponInInventory();
        SwitchWeapon(activeWeapon);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void SwitchWeapon(BaseWeapon weapon)
    {
        if (weapon != null) activeWeapon = weapon;
        //attack = activeWeapon.attack;
        //attackRange = activeWeapon.attackRange;
    }

    private BaseWeapon FindFirstWeaponInInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].type == ItemType.Weapon) return (BaseWeapon)inventory[i];
        }
        return null;
    }
}
