// ================================================================================================================================
// File:        RussianBoxCreator.cs
// Description: from https://answers.unity.com/questions/502605/terrain-section-navmesh-baking.html
//              Used to help disclude areas on the game level from the nav mesh baking
//              1) Create a box to cover an area where you don't want the navmesh to be baked at.
//              2) Apply the RussianCreateBox script to that box
//              3) Right click on the component and select "create russian boxes"
//              4) Set the navmesh layer on all generated boxes to "Navigation Static" and "not walkable"
//              5) Bake your navmesh
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class RussianBoxCreator : MonoBehaviour
{
    List<GameObject> newObjects = new List<GameObject>();

    // -------------------------------------------------------------------------
    [ContextMenu("Create Russian Boxes")]
    void CreateBoxes()
    {

        RecursiveBoxCreate(transform.localScale.y);
        for (var i = 0; i < newObjects.Count; ++i)
        {
            newObjects[i].transform.parent = gameObject.transform;
        }

    }

    // ---------------------------------------------------------------------------
    void RecursiveBoxCreate(float scale)
    {

        if (scale > 0f)
        {

            GameObject child = Instantiate(gameObject, transform.position, transform.rotation) as GameObject;
            child.transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
            newObjects.Add(child);
            // You may need to tweak the value here so that it is less than your nav height setting
            RecursiveBoxCreate(scale - 0.5f);
        }

    }
}
