using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public BaseUnit UnitPrefab;

    // for menus
    public string Description;
    public Sprite MenuSprite;
}

public enum Faction
{
    Hero = 0,
    Enemy = 1
}