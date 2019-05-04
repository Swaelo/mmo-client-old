// ================================================================================================================================
// File:        ItemManager.cs
// Description: Keeps track of all the active item pickup objects currently active in the game world
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    //Singleton class so it can be easily accessed anywhere in the codebase
    public static ItemManager Instance = null;
    private void Awake() { Instance = this; }

    //Store all the games active item pickups in a dictionary indexed by their unique network ID number
    public Dictionary<int, GameObject> ActiveItemDictionary = new Dictionary<int, GameObject>();

    //Returns all the active item pickups in a list
    public List<GameObject> GetActiveItems()
    {
        //Create a new list to store all the active items
        List<GameObject> ActiveItems = new List<GameObject>();

        //Loop through every value in the item dictionary, adding each object into the active item list
        foreach (KeyValuePair<int, GameObject> ActiveItem in ActiveItemDictionary)
            ActiveItems.Add(ActiveItem.Value);

        //Return the final list of all the active item pickups
        return ActiveItems;
    }

    //Adds a new pickup item into the game world
    public void AddItemPickup(int ItemNumber, int ItemID, Vector3 ItemLocation)
    {
        //Fetch the pickup prefab for the new pickup object being added, then use it to spawn a new item pickup object into the game world
        GameObject PickupPrefab = PickupPrefabs.Instance.GetPickupPrefab(ItemList.Instance.GetItemName(ItemNumber));
        GameObject NewItemPickup = Instantiate(PickupPrefab, ItemLocation, Quaternion.identity);

        //Store all the important item data inside the new pickups Item component
        Item NewItemData = NewItemPickup.GetComponent<Item>();
        NewItemData.ItemID = ItemID;

        //Keep the new item pickup in a list with all the other pickup objects
        ActiveItemDictionary.Add(ItemID, NewItemPickup);
    }

    //Removes one of the current pickup items from the game world
    public void RemoveItemPickup(int ItemID)
    {
        //Fetch the game object from the dictionary
        GameObject ItemPickup = ActiveItemDictionary[ItemID];

        //Remove it from the dictionary list and destroy it
        ActiveItemDictionary.Remove(ItemID);
        GameObject.Destroy(ItemPickup);
    }
}
