using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    //item manager is a singleton class so it can easily be access wherever it needs to be used
    public static ItemManager Instance = null;
    private void Awake() { Instance = this; }

    //Keep a list of item pickups which are currently active in the game world
    public List<GameObject> ActiveItems = new List<GameObject>();

    //Functions for spawning new item pickups into the game world
    public void AddConsumable(int ItemID, string ItemName, Vector3 ItemLocation)
    {
        //Fetch the correct type of consumable prefab from the prefab list
        GameObject Prefab = ConsumablePrefabs.Instance.GetConsumablePrefab(ItemName);
        //Spawn the consumable into the game world
        GameObject NewConsumable = Instantiate(Prefab, ItemLocation, Quaternion.identity);
        //Assign the id number to the item
        NewConsumable.GetComponent<Pickup>().ItemID = ItemID;
        //Store it with the rest of the active pickup items
        ActiveItems.Add(NewConsumable); 
    }

    //Adds a new piece of equipment into the game world
    public void AddEquipment(int ItemID, string ItemName, Vector3 ItemLocation)
    {
        GameObject Prefab = EquipmentPrefabs.Instance.GetEquipmentPrefab(ItemName);
        GameObject NewEquipment = Instantiate(Prefab, ItemLocation, Quaternion.identity);
        NewEquipment.GetComponent<Pickup>().ItemID = ItemID;
        ActiveItems.Add(NewEquipment);
    }

    private GameObject GetItem(int ItemID)
    {
        foreach(GameObject Item in ActiveItems)
        {
            if (Item.GetComponent<Pickup>().ItemID == ItemID)
                return Item;
        }
        return null;
    }

    public void RemoveItem(int ItemID)
    {
        GameObject OldItem = GetItem(ItemID);
        ActiveItems.Remove(OldItem);
        Destroy(OldItem);
    }
}
