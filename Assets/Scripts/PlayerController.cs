using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator AnimController;
    private Vector3 PreviousPosition;
    public GameObject PlayerCamera;
    public float WalkSpeed = 3.0f;
    public float TurnSpeed = 300f;
    private CharacterController Controller;

    private float UpdateInterval = 1.0f;
    private float NextUpdate = 1.0f;

    private void Awake()
    {
        AnimController = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        PreviousPosition = transform.position;
        PlayerCamera.transform.parent = null;
    }

    private void Update()
    {
        //Tell the animation controller how much we are moving so it knows when to play the walking animation
        float DistanceTravelled = Vector3.Distance(transform.position, PreviousPosition);
        AnimController.SetFloat("Movement", DistanceTravelled);
        PreviousPosition = transform.position;

        //Allow the player to move based on what direction the camera is facing
        float XInput = Input.GetAxis("Horizontal");
        float YInput = Input.GetAxis("Vertical");
        Vector3 MovementX = Vector3.Cross(transform.up, PlayerCamera.transform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
        Vector3 PlayerMovement = XInput * MovementX + YInput * MovementY;
        Controller.Move(PlayerMovement * WalkSpeed * Time.deltaTime);
        //Slowly rotate towards the direction of movement
        if(PlayerMovement.x != 0 || PlayerMovement.z != 0)
        {
            Quaternion NewRotation = Quaternion.LookRotation(PlayerMovement);
            Vector3 Eulers = NewRotation.eulerAngles;
            Eulers.x = transform.rotation.x;
            NewRotation.eulerAngles = Eulers;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, NewRotation, TurnSpeed * Time.deltaTime);
        }

        //Once per second, tell the server to update all other clients on this characters position
        NextUpdate -= Time.deltaTime;
        if(NextUpdate <= 0.0f)
        {
            NextUpdate = UpdateInterval;
            PacketSender.instance.SendPlayerUpdate(transform.position);
        }
    }

    //private void OnApplicationQuit()
    //{
    //    PacketSender.instance.SendDisconnectNotice();
    //}
}