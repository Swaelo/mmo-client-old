// ================================================================================================================================
// File:        DebugPlayerSwitch.cs
// Description: Quick switch between debug player or normal game play
// ================================================================================================================================

using UnityEngine;

public class DebugPlayerSwitch : MonoBehaviour
{
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject DebugPlayer;
    [SerializeField] private GameObject MenuUI;

    public bool UseDebugPlayer = true;

    private void Awake()
    {
        if (UseDebugPlayer)
            DebugPlayer.SetActive(true);
        else
        {
            MainCamera.SetActive(true);
            MenuUI.SetActive(true);
        }
    }
}
