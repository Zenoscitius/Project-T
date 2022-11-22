using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
public class Inventory : ScriptableObject
{
    public ScriptableItem[] itemArray = new ScriptableItem[5];

    public void Add(ScriptableItem item)
    {
        bool isAdded = false;

        for (int i = 0; i < itemArray.Length; i++)
        {
            if (itemArray[i] == null)
            {
                itemArray[i] = item;
                isAdded = true;
            }
        }

        if (!isAdded) Debug.Log("Inventory full, item not added");
    }

    public void Remove(ScriptableItem item)
    {
        bool isRemoved = false;

        for (int i = 0; i < itemArray.Length; i++)
        {
            if (itemArray[i] == item)
            {
                itemArray[i] = null;
                isRemoved = true;
            }
        }

        if (!isRemoved) Debug.Log("No such item in inventory");
    }

    public BaseWeapon FindFirstWeaponInInventory()
    {
        for(int i = 0; i < itemArray.Length; i++)
        {
            if (itemArray[i].Type.Equals(ItemType.Weapon)) return null;
        }
        return null;
    }
}
