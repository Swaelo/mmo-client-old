using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonPlayerController : MonoBehaviour
{
    public bool FirstPersonActive = false;
    [SerializeField] private Animator AnimationController;
    [SerializeField] private CharacterController Controller;
    [SerializeField] private float WalkSpeed = 3;
    [SerializeField] private float GravityForce = 0.1f;
    [SerializeField] private float YVelocity = 0.0f;
    [SerializeField] private float JumpHeight = 3;
    private Vector3 PreviousPosition;
    private float UpdateInterval = 1.0f;
    private float NextUpdate = 1.0f;

    public void StartFirstPersonMode()
    {
        FirstPersonActive = true;
    }
    public void EndFirstPersonMode()
    {
        FirstPersonActive = false;
    }

    private void Awake()
    {
        PreviousPosition = transform.position;
    }

    private void Update()
    {
        //Do nothing while in third person mode
        if (!FirstPersonActive)
            return;

        //Tell the animation controller how much we are moving, it will handle idle/walking animation blending
        AnimationController.SetFloat("Movement", Vector3.Distance(transform.position, PreviousPosition));

        //Player moves around with WASD, Arrow Keys or Joystick
        Vector3 MovementX = Vector3.Cross(transform.up, transform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
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
    }
}
