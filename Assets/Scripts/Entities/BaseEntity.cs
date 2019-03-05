// ================================================================================================================================
// File:        BaseEntity.cs
// Description: All types of entities must implement this base class so they can be handled by the entity manager, among other things
// Author:      Harley Laurie          
// Notes:       
// ================================================================================================================================

using UnityEngine;

public abstract class BaseEntity
{
    public GameObject GameObject;
    public string Type;
    public string ID;
    public Vector3 Position;
    public Vector3 Scale;
    public Quaternion Rotation;
    public abstract void Update(float dt);
}
