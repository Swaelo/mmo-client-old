// ================================================================================================================================
// File:        Consumable.cs
// Description: Base game item Consumable object
// ================================================================================================================================

using UnityEngine;

//This next line of code allows us to create Consumables in the Inspector
[CreateAssetMenu (fileName = "New Consumable", menuName = "Items/Consumable")]
public class Consumable : Item
{
    public Consumable()
    {
        Type = ItemType.Consumable;
    }

    //Different values can be set to these variables to define what effects the consumable will have upon the player when they consume it
    //All values are default set to zero, then redefined in specific objects to do what they must
    public int AdjustHealthValue = 0;
    public int AdjustManaValue = 0;
    public int AdjustStaminaValue = 0;

    //Override the base Use function so these adjustments can be applied to the character when they use the consumable
    public override void Use()
    {
        ////Find the player characters resource class which we will use to effect the changes of the consumable
        //PlayerResources Resources = Inventory.Instance.gameObject.GetComponent<PlayerResources>();
        ////Apply each of the consumables adjustment values upon the players resources, only those which have been defined will have any effect as the rest are default to a value of 0
        //Resources.AdjustHealth(AdjustHealthValue);
        //Resources.AdjustMana(AdjustManaValue);
        //Resources.AdjustStamina(AdjustStaminaValue);
        ////Now remove the item from the players inventory
        //Inventory.Instance.RemoveItem(this);
    }
}
