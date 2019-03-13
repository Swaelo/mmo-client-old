// ================================================================================================================================
// File:        PlayerPrefabs.cs
// Description: List of different prefabs that may be used when spawning player characters into the game world
// ================================================================================================================================

using UnityEngine;

public class PlayerPrefabs : MonoBehaviour
{
    public static PlayerPrefabs Instance;
    private void Awake() { Instance = this; }

    public GameObject ClientPlayer;
    public GameObject ExternalPlayer;
}