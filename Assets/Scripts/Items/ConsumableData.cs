// ================================================================================================================================
// File:        ConsumableData.cs
// Description: Contains information about a consumable game item, these objects are generated automatically through the ConsumableEffectsWizard custom editor window
// ================================================================================================================================

using UnityEngine;

public class ConsumableData : ScriptableObject
{
    public string EffectName = "";
    public int HealthValueAdjustment = 0;   //What type of numerical effect using this consumable has on the players currently health value
    public int StaminaValueAdjustment = 0;  //Numerical effect consumption has on the players current stamina value
    public int ManaValueAdjustment = 0; //Numerical effect consumption has on the players current mana value
}
