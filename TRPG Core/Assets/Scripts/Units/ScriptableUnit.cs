using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public BaseUnit UnitPrefab;
    public String unitName;

    // for menus
    public string Description;
    public Sprite MenuSprite;

    //exhaust
    public Boolean isExhausted;


    public void Exhaust()
    {
        isExhausted = true;
        UnitPrefab.exhaustHighlight.SetActive(true);
    }

    public void Refresh()
    {
        isExhausted = false;
        UnitPrefab.exhaustHighlight.SetActive(false);
    }

    public bool IsExhausted() { return isExhausted; }
}

public enum Faction
{
    Hero = 0,
    Enemy = 1
}