// ================================================================================================================================
// File:        ExternalPlayerMovement.cs
// Description: Relies of receiving updates from the server, moves towards whatever location the server has told us
// Author:      Harley Laurie          
// Notes:       client side predicition needs to be implemented here when combat mechanics start being implemented, thats when
//              having more accurate details will make a huge difference in the quality of the gameplay
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExternalPlayerMovement : MonoBehaviour
{
    //Components to move and animate the character
    [SerializeField] private CharacterController CharacterController;
    [SerializeField] private Animator AnimationController;

    //Position and Rotation values we should be moving towards
    private Vector3 TargetPosition;
    private Quaternion TargetRotation;

    //Calculate distance travelled each frame to sent to the animation controller
    private Vector3 PreviousPosition;

    private void Awake()
    {
        //Set default values
        TargetPosition = transform.position;
        TargetRotation = transform.rotation;
        PreviousPosition = transform.position;
    }

    private void Update()
    {
        //Calcuate distance travelled and sent it to the animation controller
        float DistanceTravelled = Vector3.Distance(transform.position, PreviousPosition);
        AnimationController.SetFloat("Movement", DistanceTravelled);
        PreviousPosition = transform.position;

        //Use the character controller to move toward the target location when we are a distance away from it
        if(Vector3.Distance(transform.position, TargetPosition) >= 0.1f)
        {
            //What direction we need to do move to reach the target location
            Vector3 TargetDirection = (TargetPosition - transform.position).normalized;
            //Move in that direction, run if we are getting too far away
            CharacterController.Move(TargetDirection * 5 * Time.deltaTime);
        }

        //Rotate towards our target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, 300 * Time.deltaTime);
    }

    public void UpdatePosition(Vector3 NewTarget, Quaternion NewRotation)
    {
        TargetPosition = NewTarget;
        TargetRotation = NewRotation;
    }
}