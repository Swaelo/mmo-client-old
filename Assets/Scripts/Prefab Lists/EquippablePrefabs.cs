// ================================================================================================================================
// File:        EquippablePrefabs.cs
// Description: 
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class EquippablePrefabs : MonoBehaviour
{
    public static EquippablePrefabs Instance;
    private void Awake() { Instance = this; }

    public GameObject[] Equippables;
    private Dictionary<string, GameObject> EquippableDictionary = new Dictionary<string, GameObject>();
    private void Start()
    {
        foreach (GameObject Equippable in Equippables)
            EquippableDictionary.Add(Equippable.transform.name, Equippable);
    }

    public GameObject GetEquippablePrefab(string EquippableName)
    {
        if(!EquippableDictionary.ContainsKey(EquippableName))
        {
            l.og(EquippableName + " key doesnt exist in the equippable prefabs dictionary.");
            return null;
        }

        return EquippableDictionary[EquippableName];
    }
}
