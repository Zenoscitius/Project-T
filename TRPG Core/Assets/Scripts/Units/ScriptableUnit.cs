using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    [SerializeField] private Stats _stats;
    public Stats BaseStats => _stats;
    public BaseUnit UnitPrefab;



    // for menus
    public string Description;
    public Sprite MenuSprite;
}

public struct Stats
{
    public int Health;
    public int AttackPower;
    public int Speed;
}

public enum Faction
{
    Hero = 0,
    Enemy = 1
}