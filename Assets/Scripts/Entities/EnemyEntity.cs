// ================================================================================================================================
// File:        EnemyEntity.cs
// Description: Defines a single enemy currently active in the servers world simulation, controls all of their AI/Behaviour during play
// ================================================================================================================================

using UnityEngine;

public enum EnemyState
{
    Idle,
    Attack,
    Flee
}

public class EnemyEntity : BaseEntity
{
    public EnemyState State = EnemyState.Idle;
    public float WalkSpeed = 500;
    public float TurnSpeed = 300;

    public EnemyEntity(string Type, string ID, Vector3 Position, Vector3 Scale, Quaternion Rotation)
    {
        this.Type = Type;
        this.ID = ID;
        this.Position = Position;
        this.Scale = Scale;
        this.Rotation = Rotation;

        //Get the correct prefab for this entity type and spawn it in the scene
        this.GameObject = GameObject.Instantiate(EntityPrefabs.GetEntityPrefab(Type), Position, Rotation);
    }

    public override void Update(float dt)
    {

    }
}
