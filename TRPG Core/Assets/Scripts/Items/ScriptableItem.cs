using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Item")]
public class ScriptableItem : ScriptableObject
{
    public ItemType Type;
    public Sprite MenuSprite;
    public string Description, ItemName;
    public int maxDurability, currentDurability;
}
public enum ItemType
{
    Weapon = 1,
    Consummable = 2,
}