// ================================================================================================================================
// File:        PlayerItemEquip.cs
// Description: Controls equipping and unequipping items from the players character
// ================================================================================================================================

using UnityEngine;

public class PlayerItemEquip : MonoBehaviour
{
    public static PlayerItemEquip Instance = null;
    private void Awake() { Instance = this; }

    //Helmet
    public GameObject HeadSlotAnchor;
    public GameObject CurrentHeadEquipment = null;
    //Necklace
    public GameObject NeckSlotAnchor;
    public GameObject CurrentNeckEquipment = null;
    //Cloak
    public GameObject CloakSlotAnchor;
    public GameObject CurrentCloakEquipment = null;
    //Shirt
    public GameObject ShirtSlotAnchor;
    public GameObject CurrentShirtEquipment = null;
    //Shoulders
    public GameObject LeftShoulderAnchor;
    public GameObject CurrentLeftShoulderEquipment = null;
    public GameObject RightShoulderAnchor;
    public GameObject CurrentRightShoulderEquipment = null;
    //Pants
    public GameObject PantsSlotAnchor;
    public GameObject CurrentPantsEquipment = null;
    //Feet
    public GameObject LeftFootSlotAnchor;
    public GameObject CurrentLeftFootEquipment = null;
    public GameObject RightFootSlotAnchor;
    public GameObject CurrentRightFootEquipment = null;
    //Gloves
    public GameObject LeftGloveSlotAnchor;
    public GameObject CurrentLeftGloveEquipment = null;
    public GameObject RightGloveSlotAnchor;
    public GameObject CurrentRightGloveEquipment = null;
    //Weapons/Shield
    public GameObject LeftHandSlotAnchor;
    public GameObject CurrentLeftHandEquipment = null;
    public GameObject RightHandSlotAnchor;
    public GameObject CurrentRightHandEquipment = null;

    //Moves and parents the new equipment item to its anchor object
    private void AnchorItem(GameObject NewEquipment, GameObject EquipmentAnchor)
    {
        NewEquipment.transform.position = EquipmentAnchor.transform.position;
        NewEquipment.transform.parent = EquipmentAnchor.transform;
    }

    //Removes any equipped item from the specificed equipment slot
    public void UnequipItem(EquipmentSlot EquipSlot)
    {
        switch(EquipSlot)
        {
            //Helmet Slot
            case (EquipmentSlot.Head):
                Destroy(CurrentHeadEquipment);
                CurrentHeadEquipment = null;
                break;

            //Necklace Slot
            case (EquipmentSlot.Neck):
                Destroy(CurrentNeckEquipment);
                CurrentNeckEquipment = null;
                break;

            //Cloak Slot
            case (EquipmentSlot.Back):
                Destroy(CurrentCloakEquipment);
                CurrentCloakEquipment = null;
                break;

            //Shirt Slot
            case (EquipmentSlot.Chest):
                Destroy(CurrentShirtEquipment);
                CurrentShirtEquipment = null;
                break;

            //Shoulder Slots
            case (EquipmentSlot.LeftShoulder):
                Destroy(CurrentLeftShoulderEquipment);
                CurrentLeftShoulderEquipment = null;
                break;
            case (EquipmentSlot.RightShoulder):
                Destroy(CurrentRightShoulderEquipment);
                CurrentRightShoulderEquipment = null;
                break;

            //Pants Slot
            case (EquipmentSlot.Legs):
                Destroy(CurrentPantsEquipment);
                CurrentPantsEquipment = null;
                break;

            //Feet Slots
            case (EquipmentSlot.LeftFoot):
                Destroy(CurrentLeftFootEquipment);
                CurrentLeftFootEquipment = null;
                break;
            case (EquipmentSlot.RightFoot):
                Destroy(CurrentRightFootEquipment);
                CurrentRightFootEquipment = null;
                break;

            //Glove Slots
            case (EquipmentSlot.LeftGlove):
                Destroy(CurrentLeftGloveEquipment);
                CurrentLeftGloveEquipment = null;
                break;
            case (EquipmentSlot.RightGlove):
                Destroy(CurrentRightGloveEquipment);
                CurrentRightGloveEquipment = null;
                break;

            //Hand Slots
            case (EquipmentSlot.LeftHand):
                Destroy(CurrentLeftHandEquipment);
                CurrentLeftHandEquipment = null;
                break;
            case (EquipmentSlot.RightHand):
                Destroy(CurrentRightHandEquipment);
                CurrentRightHandEquipment = null;
                break;
        }
    }

    //Equips the given item to the specified equipment slot
    public void EquipItem(EquipmentSlot EquipSlot, string ItemName)
    {
        //Instantiate the new equipment item that is being equipped onto the character
        GameObject NewEquipment = Instantiate(EquippablePrefabs.Instance.GetEquippablePrefab(ItemName));

        //Figure out where this item is supposed to be equipped to on the player
        switch (EquipSlot)
        {
            //Helmet Slot
            case (EquipmentSlot.Head):
                AnchorItem(NewEquipment, HeadSlotAnchor);
                CurrentHeadEquipment = NewEquipment;
                break;

            //Necklace Slot
            case (EquipmentSlot.Neck):
                AnchorItem(NewEquipment, NeckSlotAnchor);
                CurrentNeckEquipment = NewEquipment;
                break;

            //Cloak Slot
            case (EquipmentSlot.Back):
                AnchorItem(NewEquipment, CloakSlotAnchor);
                CurrentCloakEquipment = NewEquipment;
                break;

            //Shirt Slot
            case (EquipmentSlot.Chest):
                AnchorItem(NewEquipment, ShirtSlotAnchor);
                CurrentShirtEquipment = NewEquipment;
                break;

            //Shoulder Slots
            case (EquipmentSlot.LeftShoulder):
                AnchorItem(NewEquipment, LeftShoulderAnchor);
                CurrentLeftShoulderEquipment = NewEquipment;
                break;
            case (EquipmentSlot.RightShoulder):
                AnchorItem(NewEquipment, RightShoulderAnchor);
                CurrentRightShoulderEquipment = NewEquipment;
                break;

            //Pants Slot
            case (EquipmentSlot.Legs):
                AnchorItem(NewEquipment, PantsSlotAnchor);
                CurrentPantsEquipment = NewEquipment;
                break;

            //Feet Slots
            case (EquipmentSlot.LeftFoot):
                AnchorItem(NewEquipment, LeftFootSlotAnchor);
                CurrentLeftFootEquipment = NewEquipment;
                break;
            case (EquipmentSlot.RightFoot):
                AnchorItem(NewEquipment, RightFootSlotAnchor);
                CurrentRightFootEquipment = NewEquipment;
                break;

            //Glove Slots
            case (EquipmentSlot.LeftGlove):
                AnchorItem(NewEquipment, LeftGloveSlotAnchor);
                CurrentLeftGloveEquipment = NewEquipment;
                break;
            case (EquipmentSlot.RightGlove):
                AnchorItem(NewEquipment, RightGloveSlotAnchor);
                CurrentRightGloveEquipment = NewEquipment;
                break;

            //Hand Slots
            case (EquipmentSlot.LeftHand):
                AnchorItem(NewEquipment, LeftHandSlotAnchor);
                CurrentLeftHandEquipment = NewEquipment;
                break;
            case (EquipmentSlot.RightHand):
                AnchorItem(NewEquipment, RightHandSlotAnchor);
                CurrentRightHandEquipment = NewEquipment;
                break;
        }
    }
}