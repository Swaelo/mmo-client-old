// ================================================================================================================================
// File:        ServerEntity.cs
// Description: Stores all of the information regarding a single entity active in the game world that the server
//              has told us to keep track of
// Author:      Harley Laurie          
// Notes:       
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEntity : MonoBehaviour
{
    public string ID = "-1";
    public string Type = "N/A";

    public Vector3 TargetPosition;
    public Quaternion TargetRotation;

    public void UpdatePosition(Vector3 NewTarget, Quaternion NewRotation)
    {
        TargetPosition = NewTarget;
        TargetRotation = NewRotation;
    }
}
