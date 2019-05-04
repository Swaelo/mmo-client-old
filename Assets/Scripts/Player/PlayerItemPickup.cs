// ================================================================================================================================
// File:        PlayerItemPickup.cs
// Description: Allows the player to pick up items off the ground, placing them into the characters inventory
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerItemPickup : MonoBehaviour
{
    public GameObject PlayerCamera;
    public GameObject PlayerPickupEffect;
    public TextMeshPro ItemNameDisplay;

    public float MaxPickupDistance = 3f;
    private int NonPlayerLayerMask = ~(1 << 10);   //Layer 10 is the players layer, we want to cast a ray against all other layers

    //Whatever item the players camera is aiming at, that they are able to pickup
    private bool ItemHighlighted = false;
    private GameObject HighlightedItem = null;

    private void Awake()
    {
        PlayerPickupEffect.SetActive(false);
    }

    private void LateUpdate()
    {
        //Find where the player camera is looking, ignore the players layer so the ray hits the environment where they are looking
        RaycastHit RayHit;
        if(Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.TransformDirection(Vector3.forward), out RayHit, Mathf.Infinity, NonPlayerLayerMask))
        {
            //Get the current list of item pickups active in the game world
            List<GameObject> ItemPickups = ItemManager.Instance.GetActiveItems();

            //Now create a new list, containing only the items that are within the players pickup range
            List<GameObject> ItemsInRange = new List<GameObject>();
            foreach(GameObject Item in ItemPickups)
            {
                //Check the distance of each item from the current location where the player is aiming their camera
                float ItemDistance = Vector3.Distance(RayHit.point, Item.transform.position);

                //Any items that are close enough to be picked up by the player are added to list
                if (ItemDistance <= MaxPickupDistance)
                    ItemsInRange.Add(Item);
            }

            //Now, if there are any pickup objects in range, allow the plaeyr to pickup whichever one is closest
            if(ItemsInRange.Count > 0)
            {
                //Find which item in the list is closest
                GameObject ClosestItem = GameObjectFinder.FindClosest(RayHit.point, ItemsInRange);

                //Enable the item pickup sparkle effect, place it at this items location, then display the items name above it
                PlayerPickupEffect.SetActive(true);
                PlayerPickupEffect.transform.position = ClosestItem.transform.position;
                ItemNameDisplay.text = ClosestItem.GetComponent<Item>().Data.DisplayName;

                //Now remember that this is the current highlighted item, so we can then allow the player to pick it up
                ItemHighlighted = true;
                HighlightedItem = ClosestItem;
            }
            //Otherwise, if there are no items in the players pickedup range, disable the pickup effect and note that nothing is highlighted
            else
            {
                PlayerPickupEffect.SetActive(false);
                ItemHighlighted = false;
                HighlightedItem = null;
            }
        }
    }

    private void Update()
    {
        //If the player currently has an item highlighted then allow them to pick it up with the E key
        if(ItemHighlighted && Input.GetKeyDown(KeyCode.E))
        {
            //Tell the game server we want to pick this item up
            ItemData SelectedItem = HighlightedItem.GetComponent<Item>().Data;
            PacketManager.Instance.SendTakeItemRequest(PlayerManager.Instance.GetCurrentPlayerName(), SelectedItem, HighlightedItem.GetComponent<Item>().ItemID);
        }
    }
}
