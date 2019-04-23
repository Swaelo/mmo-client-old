// ================================================================================================================================
// File:        UIManager.cs
// Description: 
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    //UIManager is a singleton so i can be accessed easily through the code base
    public static UIManager Instance = null;
    private void Awake() { Instance = this; }
    
    //Complete set of UI components to be managed, indexed in dictionary by their name
    public GameObject[] UIPanelObjects;
    private Dictionary<string, GameObject> UIPanels = new Dictionary<string, GameObject>();
    private void Start()
    {
        foreach (GameObject UIPanel in UIPanelObjects)
            UIPanels.Add(UIPanel.transform.name, UIPanel);
    }

    //Helper functions for easily enabling and disabling parts of the user interface
    public void TogglePanelDisplay(string PanelName, bool ShouldDisplay)
    {
        UIPanels[PanelName].SetActive(ShouldDisplay);
    }

    public void HideAllPanels()
    {
        foreach (GameObject UIPanel in UIPanelObjects)
            UIPanel.SetActive(false);
    }
}
