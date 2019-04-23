// ================================================================================================================================
// File:        EntityManager.cs
// Description: Keeps track of all the entities the server has told us about
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    //Current list of all entities in the game right now
    public static Dictionary<string, GameObject> ActiveEntities = new Dictionary<string, GameObject>();

    //Stores a new entity into the dictionary
    public static void AddNewEntity(string ID, GameObject Entity)
    {
        ActiveEntities.Add(ID, Entity);
    }

    //Returns all the entities in a List
    public static List<GameObject> GetEntityList()
    {
        List<GameObject> EntityList = new List<GameObject>();
        foreach(var Entity in ActiveEntities)
            EntityList.Add(Entity.Value);
        return EntityList;
    }

    //Finds the right entity and delivers their updates to them
    public static void UpdateEntity(string EntityID, Vector3 EntityPosition, Quaternion EntityRotation, int EntityHealth)
    {
        //Make sure the given entity actually exists
        if (!ActiveEntities.ContainsKey(EntityID))
            return;

        //Use the dictionary to quickly find the entity who needs updating
        ServerEntity OutOfDateEntity = ActiveEntities[EntityID].GetComponent<ServerEntity>();
        OutOfDateEntity.UpdatePosition(EntityPosition, EntityRotation);
        OutOfDateEntity.UpdateHealth(EntityHealth);
    }

    //Removes an entity from the game
    public static void RemoveEntity(string EntityID)
    {
        GameObject Entity = ActiveEntities[EntityID];
        ActiveEntities.Remove(EntityID);
        Destroy(Entity);
    }
}
