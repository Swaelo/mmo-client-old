// ================================================================================================================================
// File:        ConsumablePrefabs.cs
// Description: Allows easy access to any consumable prefab items that exist in the game
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class ConsumablePrefabs : MonoBehaviour
{
    //This is a singleton instance so it can easily be accessed wherever it needs to be used
    public static ConsumablePrefabs Instance;
    private void Awake() { Instance = this; }

    //The complete list of consumable items available in the game
    public GameObject[] Consumables;
    //During runtime each consumable is indexed into a dictionary by its name
    private Dictionary<string, GameObject> ConsumableDictionary = new Dictionary<string, GameObject>();
    private void Start()
    {
        foreach (GameObject Consumable in Consumables)
            ConsumableDictionary.Add(Consumable.transform.name, Consumable);
    }

    //Returns a consumable prefab by its name from the dictionary
    public GameObject GetConsumablePrefab(string ConsumableName)
    {
        //Make sure the dictionary contains an item by this name
        if(!ConsumableDictionary.ContainsKey(ConsumableName))
        {
            l.og(ConsumableName + " key does not exist in the consumable prefabs dictionary.");
            return null;
        }

        return ConsumableDictionary[ConsumableName];
    }
}
