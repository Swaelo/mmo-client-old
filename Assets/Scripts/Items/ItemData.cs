// ================================================================================================================================
// File:        ItemData.cs
// Description: Contains information about a game item, these objects are generated automatically through the ItemData Wizard Window
// ================================================================================================================================

using UnityEngine;

public class ItemData : ScriptableObject
{
    public string Name = "";
    public string DisplayName = "";
    public string Description = "";
    public ItemType Type = ItemType.NULL;
    public EquipmentSlot Slot = EquipmentSlot.NULL;
    public GameObject Prefab = null;
    public Sprite Icon = null;
    public int ItemNumber = 0;
    public int ItemID = 0;
    public ItemRarity Rarity = ItemRarity.Common;

    public virtual void Use()
    {

    }
}
