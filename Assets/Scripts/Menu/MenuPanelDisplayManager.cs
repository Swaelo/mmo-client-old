// ================================================================================================================================
// File:        MenuPanelDisplayManager.cs
// Description: Handles the quick switching to automatically toggle between different panels in the main system menu
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class MenuPanelDisplayManager : MonoBehaviour
{
    //This class follows singleton design so it can easily be accessed from anywhere in the code to make menu transitions easily
    public static MenuPanelDisplayManager Instance = null;
    private void Awake() { Instance = this; }

    public GameObject[] MenuPanelObjects;   //All panel objects should be stored in this array through the inspector menu, they will be populated into the dictionary when the game starts
    private Dictionary<string, GameObject> MenuPanelDictionary = new Dictionary<string, GameObject>();  //Panel objects sorted into dictionary by their game object names in the unity inspectory

    //Logo and title to be hidden once the game has been entered into
    public GameObject GameLogoObject;
    public GameObject GameTitleObject;
    
    private void Start()
    {
        //When the program starts all menu panel objects are sorted into the dictionary with the game object being the dictionary address/reference
        foreach(GameObject PanelObject in MenuPanelObjects)
            MenuPanelDictionary.Add(PanelObject.transform.name, PanelObject);

        //Initially have all UI components hidden, enable them as needed
        HideAllPanels();
        HideGameTitle();
    }

    public void DisplayGameTitle()
    {
        GameLogoObject.SetActive(true);
        GameTitleObject.SetActive(true);
    }

    //Hides all menu panels from being displayed
    public void HideAllPanels()
    {
        foreach (GameObject PanelObject in MenuPanelObjects)
            PanelObject.SetActive(false);
    }

    public void HideGameTitle()
    {
        GameLogoObject.SetActive(false);
        GameTitleObject.SetActive(false);
    }

    //Displays the menu panel with the given name and hides all the others so only the desired panel can be seen
    public void DisplayPanel(string PanelName)
    {
        HideAllPanels();
        MenuPanelDictionary[PanelName].SetActive(true);
    }
}
