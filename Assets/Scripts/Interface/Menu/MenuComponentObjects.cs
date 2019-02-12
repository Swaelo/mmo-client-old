using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuComponentObjects : MonoBehaviour
{
    [SerializeField] private GameObject[] ComponentObjects;
    private Dictionary<string, GameObject> ComponentObjectDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        //Save all the component objects into the dictionary by their names
        for (int i = 0; i < ComponentObjects.Length; i++)
            ComponentObjectDictionary.Add(ComponentObjects[i].transform.name, ComponentObjects[i]);
    }

    //Toggles on/off the visibility of one of our component objects
    public void ToggleComponentObject(string ObjectName, bool ObjectStatus)
    {
        ComponentObjectDictionary[ObjectName].SetActive(ObjectStatus);
    }

    //Returns one of our component objects
    public GameObject GetComponentObject(string ObjectName)
    {
        return ComponentObjectDictionary[ObjectName];
    }

    //Toggles on everything but the named object in the parent menu objecet
    public void ToggleAllBut(string TargetObject, bool ShowTargetObject)
    {
        for(int i = 0; i < ComponentObjects.Length; i++)
        {
            //Check its name
            string ComponentName = ComponentObjects[i].transform.name;
            //Toggle the target object when we find it
            if (ComponentName == TargetObject)
                ComponentObjects[i].SetActive(ShowTargetObject);
            //Give all the other objects the opposite
            else
                ComponentObjects[i].SetActive(!ShowTargetObject);
        }
    }

    public void ViewAllButXAndY(string TargetObjectX, string TargetObjectY)
    {
        for (int i = 0; i < ComponentObjects.Length; i++)
        {
            //Check its name
            string ComponentName = ComponentObjects[i].transform.name;
            //Hide the target objects when we find them
            if (ComponentName == TargetObjectX || ComponentName == TargetObjectY)
                ComponentObjects[i].SetActive(false);
            //Display everything else
            else
                ComponentObjects[i].SetActive(!true);
        }
    }
}
