// ================================================================================================================================
// File:        PlayerCharacterController.cs
// Description: Allows the user full control of their player character, first person and third person available
// Author:      Harley Laurie          
// Notes:       
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerControllerState
{
    FirstPersonMode = 1,    //Controls the player from a first person view perspective
    ThirdPersonMode = 2,    //Controls the player from a third person over the shoulder view perspective
    Disabled = 3    //Controls need to be disabled at certain times
}

public class PlayerCharacterController : MonoBehaviour
{
    //The player defaults to third person view mode when they first enter the scene
    public PlayerControllerState ControllerState = PlayerControllerState.ThirdPersonMode;
    public PlayerControllerState PreviousState = PlayerControllerState.ThirdPersonMode;

    //Movement variables
    [SerializeField] private float WalkSpeed = 3;
    [SerializeField] private float RunSpeed = 6;
    [SerializeField] private float TurnSpeed = 300;
    [SerializeField] private float GravityForce = 0.1f;
    [SerializeField] private float YVelocity = 0.0f;
    [SerializeField] private float JumpHeight = 3;

    //Player Camera
    public Transform CameraTransform;
    [SerializeField] private PlayerCameraController CameraController;

    //Other component references that will be used by this script
    [SerializeField] private CharacterController ControllerComponent;
    [SerializeField] private Animator AnimatorComponent;

    //Previous position is remember so distance travelled between each frame can be calculated to see how fast we are moving
    //Movement speed is then sent to the animator controller to have it blend between idle and walking animation
    private Vector3 PreviousFramePosition;

    //Every quarter of a second, our new position value is sent to the server if it has changed since the last time we sent it
    //The server then shared this information with all other players so everyone can correctly see where eachother is in the world
    private Vector3 PreviousUpdatePosition;
    private Quaternion PreviousUpdateRotation;
    private float PositionUpdateInterval = 0.25f;
    private float NextPositionUpdate = 0.25f;

    private void Awake()
    {
        PlayerInfo.PlayerObject = this.gameObject;
        //The player character and their camera are grouped together as a single prefab when spawned into the scene
        //We need to immediately unparent the camera object from the player so it can move freely on its own
        CameraTransform.parent = null;
        PreviousFramePosition = transform.position;
        PreviousUpdatePosition = transform.position;
        PreviousUpdateRotation = transform.rotation;
    }

    private void Update()
    {
        //Execute the correct controller function depending on what mode is currently active
        switch(ControllerState)
        {
            case (PlayerControllerState.FirstPersonMode):
                FirstPersonMode();
                break;
            case (PlayerControllerState.ThirdPersonMode):
                ThirdPersonMode();
                break;
        }

        //Calculate distance travelled and send it to the animator controller
        float DistanceTravelled = Vector3.Distance(transform.position, PreviousFramePosition);
        PreviousFramePosition = transform.position;
        AnimatorComponent.SetFloat("Movement", DistanceTravelled);

        //Keep the server up to date regarding where our character is positioned in the game world
        NextPositionUpdate -= Time.deltaTime;
        if(NextPositionUpdate <= 0.0f)
        {
            //Reset the timer every 0.25 seconds
            NextPositionUpdate = PositionUpdateInterval;
            //If our position or rotation has changed since the last time we told the server then we need to tell it again
            if(transform.position != PreviousUpdatePosition || transform.rotation != PreviousUpdateRotation)
            {
                //Tell the server our new position value
                PacketSender.Instance.SendPlayerUpdate(transform.position, transform.rotation);
                //Now remember what we last told the server our values where
                PreviousUpdatePosition = transform.position;
                PreviousUpdateRotation = transform.rotation;
            }
        }
    }

    //This function controls the player character during third person mode
    private void ThirdPersonMode()
    {
        //Get all input from the user, what direction they want to move, how fast they want to move etc.
        float XInput = Input.GetAxis("Horizontal"); //left/right movement
        float YInput = Input.GetAxis("Vertical");   //forward/back movement
        float MovementSpeed = Input.GetKey(KeyCode.LeftShift) ? RunSpeed : WalkSpeed;   //run/walk

        //Third person movement vectors are relative to whatever direction the camera is facing at that time
        Vector3 MovementX = Vector3.Cross(transform.up, CameraTransform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
        //Combine these vectors with the players input to create a movement vector for the character controller
        Vector3 MovementVector = XInput * MovementX + YInput * MovementY;
        //Adjust vertical velocity value based on jumping input and gravity force
        if (ControllerComponent.isGrounded && Input.GetKey(KeyCode.Space))
            YVelocity = JumpHeight; //set velocity to jump height when on the ground and pressing the jump key
        if (!ControllerComponent.isGrounded)
            YVelocity -= GravityForce;  //apply gravity to velocity whilst in the air
        MovementVector.y += YVelocity;  //apply velocity to movement vector

        //Now apply the movement vector to the player
        ControllerComponent.Move(MovementVector * MovementSpeed * Time.deltaTime);

        //Slowly rotate the character to face toward the direction they are moving toward, unless no input was given
        if(MovementVector.x != 0 || MovementVector.z != 0)
        {
            //Find the rotation the player would have if they were facing directly in the the direction they are moving
            Quaternion TargetRotation = Quaternion.LookRotation(MovementVector);
            Vector3 Eulers = TargetRotation.eulerAngles;
            Eulers.x = transform.rotation.x;
            TargetRotation.eulerAngles = Eulers;
            //Now rotate slowly toward that target rotation value, so the player smoothly turns around while moving
            transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, TurnSpeed * Time.deltaTime);
        }
    }

    //This function controls the player character during first person mode
    private void FirstPersonMode()
    {
        //Get all input from the user, what direction they want to move, how fast they want to move etc.
        float XInput = Input.GetAxis("Horizontal"); //left/right movement
        float YInput = Input.GetAxis("Vertical");   //forward/back movement
        float MovementSpeed = Input.GetKey(KeyCode.LeftShift) ? RunSpeed : WalkSpeed;   //run/walk
        bool Jump = ControllerComponent.isGrounded && Input.GetKey(KeyCode.Space);  //jump

        //First person movement vectors are relative to the players current facing direction
        Vector3 MovementX = Vector3.Cross(transform.up, transform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
        Vector3 MovementVector = XInput * MovementX + YInput * MovementY;
        //Adjust vertical velocity and apply it to our movement vector
        YVelocity = Jump ? JumpHeight : YVelocity;
        YVelocity -= ControllerComponent.isGrounded ? 0 : GravityForce;
        MovementVector.y += YVelocity;

        //Now apply the movement vector to the player
        ControllerComponent.Move(MovementVector * MovementSpeed * Time.deltaTime);
    }
}
