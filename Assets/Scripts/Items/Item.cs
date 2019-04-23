using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : ScriptableObject
{
    public string Name;
    public ItemType Type;
    public EquipmentSlot Slot;
    public Sprite Icon;

    public int ID;

    public virtual void Use()
    {

    }
}
