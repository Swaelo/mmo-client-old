// ================================================================================================================================
// File:        PlayerEquipmentControl.cs
// Description: Allows the player to equip and unequip items to their character
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentControl : MonoBehaviour
{
    //Singleton for easy access
    public static PlayerEquipmentControl Instance = null;
    private void Awake() { Instance = this; }
    
    private Dictionary<string, GameObject> EquipmentPanels = new Dictionary<string, GameObject>();

    private void Start()
    {
        EquipmentPanels.Add("Head Slot", GameObject.Find("Head Slot"));
        EquipmentPanels.Add("Neck Slot", GameObject.Find("Neck Slot"));
        EquipmentPanels.Add("Back Slot", GameObject.Find("Back Slot"));
        EquipmentPanels.Add("Chest Slot", GameObject.Find("Chest Slot"));
        EquipmentPanels.Add("Right Shoulder Slot", GameObject.Find("Right Shoulder Slot"));
        EquipmentPanels.Add("Left Shoulder Slot", GameObject.Find("Left Shoulder Slot"));
        EquipmentPanels.Add("Right Glove Slot", GameObject.Find("Right Glove Slot"));
        EquipmentPanels.Add("Left Glove Slot", GameObject.Find("Left Glove Slot"));
        EquipmentPanels.Add("Legs Slot", GameObject.Find("Legs Slot"));
        EquipmentPanels.Add("Left Hand Slot", GameObject.Find("Left Hand Slot"));
        EquipmentPanels.Add("Right Hand Slot", GameObject.Find("Right Hand Slot"));
        EquipmentPanels.Add("Left Foot Slot", GameObject.Find("Left Foot Slot"));
        EquipmentPanels.Add("Right Foot Slot", GameObject.Find("Right Foot Slot"));
    }

    public GameObject GetEquipmentPanel(string PanelName)
    {
        return EquipmentPanels[PanelName];
    }

    public GameObject GetEquipmentPanel(EquipmentSlot Slot)
    {
        switch(Slot)
        {
            case (EquipmentSlot.Head):
                return EquipmentPanels["Head Slot"];
            case (EquipmentSlot.Neck):
                return EquipmentPanels["Neck Slot"];
            case (EquipmentSlot.Back):
                return EquipmentPanels["Back Slot"];
            case (EquipmentSlot.Chest):
                return EquipmentPanels["Chest Slot"];
            case (EquipmentSlot.RightShoulder):
                return EquipmentPanels["Right Shoulder Slot"];
            case (EquipmentSlot.LeftShoulder):
                return EquipmentPanels["Left Shoulder Slot"];
            case (EquipmentSlot.RightGlove):
                return EquipmentPanels["Right Glove Slot"];
            case (EquipmentSlot.LeftGlove):
                return EquipmentPanels["Left Glove Slot"];
            case (EquipmentSlot.Legs):
                return EquipmentPanels["Legs Slot"];
            case (EquipmentSlot.LeftHand):
                return EquipmentPanels["Left Hand Slot"];
            case (EquipmentSlot.RightHand):
                return EquipmentPanels["Right Hand Slot"];
            case (EquipmentSlot.LeftFoot):
                return EquipmentPanels["Left Foot Slot"];
            case (EquipmentSlot.RightFoot):
                return EquipmentPanels["Right Foot Slot"];
        }
        return null;
    }

    public void UnequipItem(string ItemSlotName)
    {
        //First check to make sure the player has space in their inventory to store the item after they unequip it
        if (PlayerInventoryControl.Instance.IsInventoryFull())
            return;

        //Send a request to the game server to have this item moved out of the players equipment and into their inventory
        PacketManager.Instance.SendUnequipItemRequest(PlayerManager.Instance.GetCurrentPlayerName(), GetEquipmentPanel(ItemSlotName).GetComponent<EquipSlot>().GearSlotType);
    }
}
