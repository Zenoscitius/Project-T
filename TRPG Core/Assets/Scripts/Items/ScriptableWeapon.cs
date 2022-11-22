using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Weapon")]
public class ScriptableWeapon : ScriptableItem
{
    public int attack;
    public int attackRange;
    public WeaponType weaponType;
}
public enum WeaponType
{
    Sword = 1,
    Spear = 2,
    Axe = 3,
    Bow = 4,
    Spell = 5,
}
