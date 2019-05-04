// ================================================================================================================================
// File:        PickupPrefabs.cs
// Description: Stores a list of all the item pickup prefabs used when adding new item picks into the game world
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPrefabs : MonoBehaviour
{
    //This is a singleton instance so it can easily be accessed wherever it needs to be used
    public static PickupPrefabs Instance;
    private void Awake() { Instance = this; }

    //Store every pickup prefab object in a dictionary, indexed by the items name
    public GameObject[] Pickups;
    private Dictionary<string, GameObject> PickupsDictionary = new Dictionary<string, GameObject>();
    
    private void Start()
    {
        //Fill the dictionary with everything in the Pickups list
        foreach (GameObject Pickup in Pickups)
            PickupsDictionary.Add(Pickup.transform.name, Pickup);
    }

    //Returns an items pickup prefab from just giving its name
    public GameObject GetPickupPrefab(string PickupName)
    {
        if (!PickupsDictionary.ContainsKey(PickupName))
        {
            l.og(PickupName + " key doesnt exist in PickupsDictionary.");
            return null;
        }

        return PickupsDictionary[PickupName];
    }
}
