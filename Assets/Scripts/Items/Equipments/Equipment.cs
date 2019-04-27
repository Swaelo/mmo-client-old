// ================================================================================================================================
// File:        Equipment.cs
// Description: Base game equippable item object
// ================================================================================================================================

using UnityEngine;

[CreateAssetMenu (fileName = "New Equipment", menuName = "Items/Equipment")]
public class Equipment : ItemData
{
    public Equipment()
    {
        Type = ItemType.Equipment;
    }

    //Physical protection granted by wearing this item
    public int ArmourValue = 1;

    //Elemental resistances granted by wearing this item
    public int FireResistance = 0;
    public int LightningResistance = 0;
    public int FrostResistance = 0;

    //Stat enhancements granted by wearing this item
    public int StrengthEnhancement = 0;
    public int AgilityEnhancement = 0;
    public int IntelligenceEnhancement = 0;
}
