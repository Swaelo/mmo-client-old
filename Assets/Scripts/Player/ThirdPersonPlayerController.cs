using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerController : MonoBehaviour
{
    public bool ThirdPersonControllerActive = true;

    public void StartThirdPersonMode()
    {
        ThirdPersonControllerActive = true;
    }
    public void EndThirdPersonMode()
    {
        ThirdPersonControllerActive = false;
    }

    [SerializeField] private Animator AnimationController;
    [SerializeField] private CharacterController Controller;
    [SerializeField] private Transform PlayerCamera;

    [SerializeField] private float WalkSpeed = 3;
    [SerializeField] private float TurnSpeed = 300;
    [SerializeField] private float GravityForce = 0.1f;
    [SerializeField] private float YVelocity = 0.0f;
    [SerializeField] private float JumpHeight = 3;

    private Vector3 PreviousPosition;
    private float UpdateInterval = 1.0f;
    private float NextUpdate = 1.0f;

    private void Awake()
    {
        PreviousPosition = transform.position;
        PlayerCamera.transform.parent = null;
    }

    private void Update()
    {
        //Do nothing while in person person mode
        if (!ThirdPersonControllerActive)
            return;

        //Tell the animation controller how much we are moving, it will handle idle/walking animation blending
        AnimationController.SetFloat("Movement", Vector3.Distance(transform.position, PreviousPosition));

        //Figure out where the player should be moved to based on their input
        Vector3 MovementX = Vector3.Cross(transform.up, PlayerCamera.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
        //Move around with WASD, Arrow Keys or Joystick
        Vector3 PlayerMovement = Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;
        //Jump with spacebar
        if (Controller.isGrounded && Input.GetKey(KeyCode.Space))
            YVelocity = JumpHeight;
        //Apply force of gravity whilst in the air
        if (!Controller.isGrounded)
            YVelocity -= GravityForce;
        PlayerMovement.y += YVelocity;
        //Apply this movement to the character controller component
        Controller.Move(PlayerMovement * WalkSpeed * Time.deltaTime);

        //Slowly rotate to face the direction the character is moving
        if(PlayerMovement.x != 0 || PlayerMovement.z != 0)
        {
            //Find the rotation we should be facing depending on which direction we are moving and lerp towards that state
            Quaternion Rotation = Quaternion.LookRotation(PlayerMovement);
            Vector3 Eulers = Rotation.eulerAngles;
            Eulers.x = transform.rotation.x;
            Rotation.eulerAngles = Eulers;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Rotation, TurnSpeed * Time.deltaTime);
        }
    }
}
