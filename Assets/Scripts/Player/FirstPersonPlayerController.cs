using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonPlayerController : MonoBehaviour
{
    public bool FirstPersonActive = false;
    [SerializeField] private CharacterController Controller;
    Animator AnimationController;
    [SerializeField] private float WalkSpeed = 3;
    public float RunSpeed = 6;
    [SerializeField] private float GravityForce = 0.1f;
    [SerializeField] private float YVelocity = 0.0f;
    [SerializeField] private float JumpHeight = 3;

    private void Awake()
    {
        AnimationController = GetComponent<Animator>();
    }

    public void StartFirstPersonMode()
    {
        FirstPersonActive = true;
    }
    public void EndFirstPersonMode()
    {
        FirstPersonActive = false;
    }

    private void Update()
    {
        //Do nothing while in third person mode
        if (!FirstPersonActive)
            return;

        //Hold shift to run
        float MovementSpeed = Input.GetKey(KeyCode.LeftShift) ? RunSpeed : WalkSpeed;

        //Player moves around with WASD, Arrow Keys or Joystick
        Vector3 MovementX = Vector3.Cross(transform.up, transform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
        Vector3 PlayerMovement = Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;
        //Jump with spacebar
        if (Controller.isGrounded && Input.GetKey(KeyCode.Space))
        {
            YVelocity = JumpHeight;
            AnimationController.SetTrigger("Jump");
            //Send a message to the server that we jumped

        }
        //Apply force of gravity whilst in the air
        if (!Controller.isGrounded)
            YVelocity -= GravityForce;
        PlayerMovement.y += YVelocity;
        //Apply this movement to the character controller component
        Controller.Move(PlayerMovement * MovementSpeed * Time.deltaTime);
    }
}
