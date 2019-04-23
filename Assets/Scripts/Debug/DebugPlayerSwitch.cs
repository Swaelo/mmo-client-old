// ================================================================================================================================
// File:        DebugPlayerSwitch.cs
// Description: Quick switch between debug player or normal game play
// ================================================================================================================================

using UnityEngine;

public class DebugPlayerSwitch : MonoBehaviour
{
    public static DebugPlayerSwitch Instance = null;
    private void Awake() { Instance = this; }

    public GameObject MainCamera;
    public GameObject DebugPlayer;

    public bool UseDebugPlayer = true;

    private void Start()
    {
        MainCamera.SetActive(!UseDebugPlayer);
        DebugPlayer.SetActive(UseDebugPlayer);
        if (UseDebugPlayer)
        {
            MenuPanelDisplayManager.Instance.HideAllPanels();
            MenuPanelDisplayManager.Instance.HideGameTitle();
        }
    }
}
