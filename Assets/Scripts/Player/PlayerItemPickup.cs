// ================================================================================================================================
// File:        PlayerItemPickup.cs
// Description: 
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class PlayerItemPickup : MonoBehaviour
{
    public GameObject PlayerCamera;
    public GameObject PlayerPickupEffect;

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
            //Now we know the world location where the player is looking, find the list of item pickups which are in range of this location
            List<GameObject> ItemPickups = ItemManager.Instance.ActiveItems;
            List<GameObject> ItemsInRange = new List<GameObject>();
            foreach(GameObject Item in ItemPickups)
            {
                float ItemDistance = Vector3.Distance(RayHit.point, Item.transform.position);
                if (ItemDistance <= MaxPickupDistance)
                    ItemsInRange.Add(Item);
            }

            //If there are any pickup objects in range, allow the player to pickup which one is closest
            if(ItemsInRange.Count > 0)
            {
                //Find which object is closest
                GameObject ClosestItem = GameObjectFinder.FindClosest(RayHit.point, ItemsInRange);
                //Enable the pickup sparkle effect and place it at the location of this item pickup
                PlayerPickupEffect.SetActive(true);
                PlayerPickupEffect.transform.position = ClosestItem.transform.position;
                //Remember this as the highlighed item so we can allow the player to pick it up
                ItemHighlighted = true;
                HighlightedItem = ClosestItem;
            }
            else
            {
                //If there are no items in range to be picked up disable the pickup effect and set that no item is highlighted
                PlayerPickupEffect.SetActive(false);
                ItemHighlighted = false;
            }
        }
    }

    private void Update()
    {
        //If the player currently has an item highlighted then allow them to pick it up with the E key
        if(ItemHighlighted && Input.GetKeyDown(KeyCode.E))
        {
            //Tell the game server we want to pick this item up
            Pickup SelectedItem = HighlightedItem.GetComponent<Pickup>();
            PacketManager.Instance.SendTakeItemRequest(PlayerManager.Instance.LocalPlayer.CurrentCharacter.CharacterName, SelectedItem.ItemNumber, SelectedItem.ItemID);
            ItemHighlighted = false;
            PlayerPickupEffect.SetActive(false);
        }
    }
}
