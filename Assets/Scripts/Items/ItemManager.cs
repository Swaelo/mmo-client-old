using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    //item manager is a singleton class so it can easily be access wherever it needs to be used
    public static ItemManager Instance = null;
    private void Awake() { Instance = this; }
    
    //Keep a list of active item picks in a dictionary indexed by their unique network ID number
    public Dictionary<int, GameObject> ActiveItems = new Dictionary<int, GameObject>();

    //Returns all the active items in a list
    public List<GameObject> GetActiveItems()
    {
        //Define the list to store all the active items
        List<GameObject> ActiveItemList = new List<GameObject>();

        //Grab each item from the dictionary and add it to the list
        foreach (KeyValuePair<int, GameObject> ActiveItem in ActiveItems)
            ActiveItemList.Add(ActiveItem.Value);

        //Return the populated list of all the game objects
        return ActiveItemList;
    }

    //Functions for spawning new item pickups into the game world
    public void AddConsumable(int ItemNumber, int ItemID, string ItemName, Vector3 ItemLocation)
    {
        //Fetch the correct type of consumable prefab from the prefab list and spawn one of them into the game world
        GameObject Prefab = ConsumablePrefabs.Instance.GetConsumablePrefab(ItemName);
        GameObject NewConsumable = Instantiate(Prefab, ItemLocation, Quaternion.identity);

        //Store all the items data inside it
        ItemData NewItemData = NewConsumable.GetComponent<Item>().Data;
        NewItemData.ItemNumber = ItemNumber;
        NewItemData.ItemID = ItemID;
        NewItemData.Type = ItemType.Consumable;
        
        //Store it with the rest of the active pickup items
        ActiveItems.Add(ItemID, NewConsumable); 
    }

    //Adds a new piece of equipment into the game world
    public void AddEquipment(int ItemNumber, int ItemID, string ItemName, Vector3 ItemLocation)
    {
        //Grab the correct prefab and spawn one of those into the world
        GameObject Prefab = PickupPrefabs.Instance.GetPickupPrefab(ItemName);
        GameObject NewEquipment = Instantiate(Prefab, ItemLocation, Quaternion.identity);

        //Store all the items data inside it
        ItemData NewItemData = NewEquipment.GetComponent<Item>().Data;
        NewItemData.ItemNumber = ItemNumber;
        NewItemData.ItemID = ItemID;
        NewItemData.Type = ItemType.Equipment;

        //Store this in the list with all the other active items
        ActiveItems.Add(ItemID, NewEquipment);
    }

    //Removes an item pickup from the game world by its network ID number
    public void RemoveItem(int ItemID)
    {
        //Fetch the item from the dictionary
        GameObject OldItem = ActiveItems[ItemID];
        //Remove it from the dictionary, then destroy the game object to remove it from the game world
        ActiveItems.Remove(ItemID);
        GameObject.Destroy(OldItem);
    }
}
