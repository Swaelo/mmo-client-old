// ================================================================================================================================
// File:        EquippablePrefabs.cs
// Description: Stores a list of all the prefab objects instantiated into the game world when visually equipping gear onto a player character
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class EquippablePrefabs : MonoBehaviour
{
    //Singleton class to be easily access from anywhere in the code
    public static EquippablePrefabs Instance;
    private void Awake() { Instance = this; }

    //Store every equippible prefab object in a dictionary, indexed by the items names
    public GameObject[] Equippables;
    private Dictionary<string, GameObject> EquippableDictionary = new Dictionary<string, GameObject>();

    //Fill the dictionary with all the values in the Equippables list
    private void Start()
    {
        foreach (GameObject Equippable in Equippables)
            EquippableDictionary.Add(Equippable.transform.name, Equippable);
    }

    //Returns an equippible prefab object from its item name
    public GameObject GetEquippablePrefab(string EquippableName)
    {
        if(!EquippableDictionary.ContainsKey(EquippableName))
        {
            Log.PrintChatMessage(EquippableName + " key doesnt exist in the equippable prefabs dictionary.");
            return null;
        }

        return EquippableDictionary[EquippableName];
    }

    //Returns an equippable prefab object from its ItemNumber
    public GameObject GetEquippablePrefab(int ItemNumber)
    {
        return GetEquippablePrefab(ItemList.Instance.GetItemName(ItemNumber));
    }
}
