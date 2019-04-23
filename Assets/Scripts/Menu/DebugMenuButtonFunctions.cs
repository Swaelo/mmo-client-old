using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenuButtonFunctions : MonoBehaviour
{
    //Moves the current player character back to the default spawn point
    public void ClickResetPosition()
    {
        GameObject PlayerObject = PlayerManager.Instance.LocalPlayer.CurrentCharacter.CharacterObject;
        PlayerObject.transform.position = new Vector3(0, 1.5f, 0);
    }
}
