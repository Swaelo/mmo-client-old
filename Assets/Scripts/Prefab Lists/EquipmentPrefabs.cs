// ================================================================================================================================
// File:        EquipmentPrefabs.cs
// Description: Allows easy access to spawn equipment pickups into the game scene
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class EquipmentPrefabs : MonoBehaviour
{
    //This is a singleton class so it can easily be accessed and used from anywhere in the codebase
    public static EquipmentPrefabs Instance;
    private void Awake() { Instance = this; }

    //Complete list of equippable items available in the game
    public GameObject[] Equipments;
    //Sorted into a dictionary during runtime for ease of use when wanting to spawn a specific item by its name
    private Dictionary<string, GameObject> EquipmentDictionary = new Dictionary<string, GameObject>();
    private void Start()
    {
        foreach (GameObject Equipment in Equipments)
            EquipmentDictionary.Add(Equipment.transform.name, Equipment);
    }

    //Returns an equipment prefab by its name
    public GameObject GetEquipmentPrefab(string EquipmentName)
    {
        //Make sure this item exists
        if(!EquipmentDictionary.ContainsKey(EquipmentName))
        {
            l.og(EquipmentName + " key does not exist in the equipment prefabs dictionary");
            return null;
        }

        return EquipmentDictionary[EquipmentName];
    }
}
