using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPrefabs : MonoBehaviour
{
    //This is a singleton instance so it can easily be accessed wherever it needs to be used
    public static PickupPrefabs Instance;
    private void Awake() { Instance = this; }

    public GameObject[] Pickups;
    private Dictionary<string, GameObject> PickupsDictionary = new Dictionary<string, GameObject>();
    private void Start()
    {
        foreach (GameObject Pickup in Pickups)
            PickupsDictionary.Add(Pickup.transform.name, Pickup);
    }

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
