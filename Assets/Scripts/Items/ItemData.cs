// ================================================================================================================================
// File:        ItemData.cs
// Description: Contains information about a game item, these objects are generated automatically through the ItemData Wizard Window
// ================================================================================================================================

using UnityEngine;

public class ItemData : ScriptableObject
{
    public string Name = "";    //Items name with all spaces and punctuation characters removed
    public string DisplayName = ""; //Items name shown inside the game
    public string Description = ""; //Items description text
    public ItemType Type = ItemType.NULL;   //Type of item this is
    public EquipmentSlot Slot = EquipmentSlot.NULL; //Which equipment slot this item fits in
    public GameObject Prefab = null;    //The item pickup prefab
    public Sprite Icon = null;  //The icon used to rendering this object in the UI
    public int ItemNumber = 0;  //The items ItemNumber
    public ItemRarity Rarity = ItemRarity.Common;   //Items rarity value
    public ConsumableData ConsumableEffect = null;  //Effect this item has on the player when consumed
}
