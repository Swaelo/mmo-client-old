using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPrefabs : MonoBehaviour
{
    [SerializeField] private GameObject[] EntityPrefabList;

    //Organize the prefabs into a dictionary sorted by their name so they can be retrieved much more easily
    private static Dictionary<string, GameObject> PrefabDictionary = new Dictionary<string, GameObject>();
    public static GameObject GetEntityPrefab(string PrefabName) { return PrefabDictionary[PrefabName]; }
    public static EntityPrefabs Instance;
    private void Awake()
    {
        //Assign the singleton instance
        Instance = this;

        //Fill the dictionary with the prefab list
        for(int i = 0; i < EntityPrefabList.Length; i++)
        {
            GameObject PrefabObject = EntityPrefabList[i];
            string PrefabName = PrefabObject.transform.name;
            PrefabDictionary.Add(PrefabName, PrefabObject);
        }
    }
}
