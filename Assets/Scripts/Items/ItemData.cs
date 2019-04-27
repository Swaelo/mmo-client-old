// ================================================================================================================================
// File:        ItemData.cs
// Description: Defines information about a game item, generated through the editor custom windows
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemData : ScriptableObject
{
    public string Name = "";
    public ItemType Type = ItemType.NULL;
    public EquipmentSlot Slot = EquipmentSlot.NULL;
    public GameObject Prefab = null;
    public Sprite Icon = null;
    public int ItemNumber = 0;
    public int ItemID = 0;

    public virtual void Use()
    {

    }
}
