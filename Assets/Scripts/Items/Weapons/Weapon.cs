// ================================================================================================================================
// File:        Weapon.cs
// Description: Base game item weapon object
// ================================================================================================================================

using UnityEngine;

[CreateAssetMenu (fileName = "New Weapon", menuName = "Items/Weapon")]
public class Weapon : Item
{
    public Weapon()
    {
        Type = ItemType.Equipment;
    }

    //Range of physical damage applied when this weapon hits a target
    public Vector2 DamageRange = new Vector2(1, 10);

    //Range of extra elemental damage types applied when this weapon hits a target
    public Vector2 AddedFireDamage = Vector2.zero;
    public Vector2 AddedLightningDamage = Vector2.zero;
    public Vector2 AddedFrostDamage = Vector2.zero;

    //Stat Enhancements given by equipping this item
    public int StrengthEnhancement = 0;
    public int AgilityEnhancement = 0;
    public int IntelligenceEnhancement = 0;
}
