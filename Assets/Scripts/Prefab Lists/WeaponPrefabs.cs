// ================================================================================================================================
// File:        WeaponPrefabs.cs
// Description: Allows easy access to spawn weapon pickups into the game scene
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class WeaponPrefabs : MonoBehaviour
{
    //Singleton class so its easily accessible from anywhere in the codebase
    public static WeaponPrefabs Instance;
    private void Awake() { Instance = this; }

    //Complete list of weapons available in the game, sorted into a dictionary for ease of retrieval
    public GameObject[] Weapons;
    private Dictionary<string, GameObject> WeaponDictionary = new Dictionary<string, GameObject>();
    private void Start()
    {
        foreach (GameObject Weapon in Weapons)
            WeaponDictionary.Add(Weapon.transform.name, Weapon);
    }

    //Returns a weapon prefab by name
    public GameObject GetWeaponPrefab(string WeaponName)
    {
        //make sure the weapon exists
        if(!WeaponDictionary.ContainsKey(WeaponName))
        {
            l.og(WeaponName + " key does not exist in the weapon prefabs dictionary.");
            return null;
        }

        return WeaponDictionary[WeaponName];
    }
}
