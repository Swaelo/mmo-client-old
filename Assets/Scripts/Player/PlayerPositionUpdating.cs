using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionUpdating : MonoBehaviour
{
    //Every 0.25 seconds, we will send an update to the server IF our position has changed since then
    private Vector3 PreviousUpdatePosition;
    private float UpdateInterval = 0.25f;
    private float NextUpdate = 0.25f;

    //Were also going to handle the animation controller while were here
    [SerializeField] private Animator AnimationController;
    private Vector3 PreviousFramePosition;

    private void Awake()
    {
        PreviousUpdatePosition = transform.position;
        PreviousFramePosition = transform.position;
    }

    private void Update()
    {
        //Tell the animation controller how much distance the player has travelled since the last frame
        float FrameDistance = Vector3.Distance(transform.position, PreviousFramePosition);
        PreviousFramePosition = transform.position;
        AnimationController.SetFloat("Movement", FrameDistance);


        //Count down the interval timer
        NextUpdate -= Time.deltaTime;

        //Wait until the timer hits zero
        if(NextUpdate <= 0.0f)
        {
            //reset the timer
            NextUpdate = UpdateInterval;
            //If our current position is different to what was last sent to the server, when we need to update them
            if(transform.position != PreviousUpdatePosition)
            {
                //Before we update, remember what position was sent to the server
                PreviousUpdatePosition = transform.position;
                PacketSender.instance.SendPlayerUpdate(transform.position);
            }
        }
    }
}
