// ================================================================================================================================
// File:        l.cs
// Description: Defines some keyboard shortcuts to do helpful actions during testing
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shortcuts : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            PacketManager.Instance.SendPlayerPurgeItems(PlayerManager.Instance.GetCurrentPlayerName());
    }
}
