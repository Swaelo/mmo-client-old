// ================================================================================================================================
// File:        GameObjectFinder.cs
// Description: Returns from a list of object which of them is the closest to a given location
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class GameObjectFinder : MonoBehaviour
{
    //Returns which object in the List is closest to the given position
    public static GameObject FindClosest(Vector3 TargetPosition, List<GameObject> ObjectList)
    {
        //Make sure the list contains objects to check against
        if (ObjectList.Count <= 0)
            return null;

        //Find how far the first object in the list is
        GameObject ClosestObject = ObjectList[0];
        float ClosestObjectDistance = Vector3.Distance(TargetPosition, ClosestObject.transform.position);
        //Now compare it against the rest, finding any others that are closer
        for(int i = 1; i < ObjectList.Count; i++)
        {
            //Find the distance of every other object in this list
            GameObject NextObject = ObjectList[i];
            float NextObjectDistance = Vector3.Distance(TargetPosition, NextObject.transform.position);
            //Check if this is closer than any object checked thus far
            if(NextObjectDistance < ClosestObjectDistance)
            {
                //Update it if so
                ClosestObject = NextObject;
                ClosestObjectDistance = NextObjectDistance;
            }
        }

        //Return which object was found to be the closest to the target location
        return ClosestObject;
    }
}
