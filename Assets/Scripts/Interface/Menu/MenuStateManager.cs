// ================================================================================================================================
// File:        MenuStateManager.cs
// Description: Controls which section of the main menu the game is currently in
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class MenuStateManager : MonoBehaviour
{
    private static string CurrentMenuState = "Main Menu";
    [SerializeField] private GameObject[] MenuStateObjects;
    private static Dictionary<string, GameObject> MenuStateDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        //Save all the menu state objects into a dictionary by their names
        for(int i = 0; i < MenuStateObjects.Length; i++)
            MenuStateDictionary.Add(MenuStateObjects[i].transform.name, MenuStateObjects[i]);
    }

    //Change to another menu state
    public static void SetMenuState(string NewState)
    {
        
        //Disable the previous menu state objects
        MenuStateDictionary[CurrentMenuState].SetActive(false);
        //Enable and assign the new state as active
        MenuStateDictionary[NewState].SetActive(true);
        CurrentMenuState = NewState;
    }

    //Returns the currently active menu state object
    public static GameObject GetCurrentMenuStateObject()
    {
        return MenuStateDictionary[CurrentMenuState];
    }

    //Returns the MenuComponentObjects component of the target menu state object
    public static MenuComponentObjects GetMenuComponents(string MenuStateName)
    {
        GameObject MenuStateObject = MenuStateDictionary[MenuStateName];
        return MenuStateObject.GetComponent<MenuComponentObjects>();
    }
}
