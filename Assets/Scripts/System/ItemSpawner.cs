using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance = null;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject SpawnItem(GameObject Prefab, Vector3 Location, Quaternion Orientation)
    {
        return Instantiate(Prefab, Location, Orientation);
    }
}
