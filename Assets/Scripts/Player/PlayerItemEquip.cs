// ================================================================================================================================
// File:        PlayerItemEquip.cs
// Description: Controls equipping and unequipping items from the players character
// ================================================================================================================================

using UnityEngine;

public class PlayerItemEquip : MonoBehaviour
{
    public static PlayerItemEquip Instance = null;
    private void Awake() { Instance = this; }

    public GameObject HeadSlotAnchor;
    public GameObject CurrentHeadEquipment = null;

    //Checks if the specified item slot is free or not
}