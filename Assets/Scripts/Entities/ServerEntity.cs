// ================================================================================================================================
// File:        ServerEntity.cs
// Description: Stores all of the information regarding a single entity active in the game world that the server has told us to keep track of
// ================================================================================================================================

using UnityEngine;

public class ServerEntity : MonoBehaviour
{
    public string ID = "-1";
    public string Type = "N/A";

    public Vector3 TargetPosition;
    public Quaternion TargetRotation;

    public float MoveSpeed = 3f;
    public float TurnSpeed = 5f;

    public void UpdatePosition(Vector3 NewTarget, Quaternion NewRotation)
    {
        TargetPosition = NewTarget;
        TargetRotation = NewRotation;
    }

    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, TargetPosition, MoveSpeed * Time.deltaTime);

        TargetRotation.x = transform.rotation.x;
        TargetRotation.z = transform.rotation.z;
        transform.rotation = Quaternion.Lerp(transform.rotation, TargetRotation, TurnSpeed * Time.deltaTime);
    }
}
