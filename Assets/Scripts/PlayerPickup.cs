using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public GameObject LeftHandHook;
    public GameObject RightHandHook;

    public float PickupDistance = 1.5f;
    private bool HoldingItem = false;

    void Update()
    {
        ////Press E to pickup/drop items
        //if(Input.GetKeyDown(KeyCode.E))
        //{
        //    if(!HoldingItem)
        //    {
        //        //Get a list of all the items on the ground
        //        GameObject[] GroundItems = GameObject.FindGameObjectsWithTag("GrabItem");
        //        //Find which of these is closest to the player
        //        GameObject ClosestGroundItem = FindClosestObject(GroundItems);
        //        //If the player is close enough to pick up this item then it gets placed into their hand
        //        float ObjectDistance = Vector3.Distance(transform.position, ClosestGroundItem.transform.position);
        //        if (ObjectDistance <= PickupDistance)
        //        {
        //            HoldingItem = true;
        //            //Tell the server we have picked this item up
        //            PacketSender.instance.SendItemTaken(ClosestGroundItem.GetComponent<GameItem>().ItemNumber);
        //        }
        //    }
        //}
    }

    private GameObject FindClosestObject(GameObject[] ObjectList)
    {
        GameObject ClosestObject = ObjectList[0];
        float ClosestObjectDistance = Vector3.Distance(transform.position, ClosestObject.transform.position);
        for(int i = 1; i < ObjectList.Length; i++)
        {
            GameObject NextObject = ObjectList[i];
            float NextDistance = Vector3.Distance(transform.position, NextObject.transform.position);
            if(NextDistance < ClosestObjectDistance)
            {
                ClosestObject = NextObject;
                ClosestObjectDistance = NextDistance;
            }
        }
        return ClosestObject;
    }
}
