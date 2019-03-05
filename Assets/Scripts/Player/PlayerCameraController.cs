// ================================================================================================================================
// File:        PlayerCameraController.cs
// Description: Gives the player full control of their camera, both 1st and 3rd person views implemented here
// Author:      Harley Laurie          
// Notes:       
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private PlayerCharacterController CharacterController;

    //Different FOV value for first person and third person modes
    [SerializeField] private float FirstPersonFOV = 90;
    [SerializeField] private float ThirdPersonFOV = 70;

    //Different mouse sensitivity levels for first and third person modes
    [SerializeField] private float FirstPersonMouseXSpeed = 30;
    [SerializeField] private float FirstPersonMouseYSpeed = 25;
    [SerializeField] private float ThirdPersonMouseXSpeed = 30;
    [SerializeField] private float ThirdPersonMouseYSpeed = 25;
    
    //First person mode variables
    [SerializeField] private GameObject FirstPersonPositionAnchor;
    [SerializeField] private GameObject FirstPersonDirectionAnchor;
    [SerializeField] private Transform PlayerTransform;

    //Third person mode variables
    [SerializeField] private GameObject ThirdPersonCameraTarget;
    private float CurrentCameraDistance = 3.5f; //Current distance between player and camera
    private float MinimumCameraDistance = 1.0f; //The minimum allowed distance between player and camera / how far you can zoom in
    private float MaximumCameraDistance = 8.0f; //Maximum allowed distance / how far you can zoom out
    private float YMinimum = -20;   //Camera Y Rotation values must be clamped to avoid gimbal lock
    private float YMaximum = 80;
    private float CurrentX = 0f;    //Current rotation values must be tracked so they can be clamped too
    private float CurrentY = 0f;

    //Mouse cursor is locked by default by the player character when they enter the scene
    //cursor lock is disabled by hitting the escape key, then re-enabled by clicking on the game view again
    public bool CursorLocked = true;

    private bool CursorChat = false;    //If the cursor is currently being controlled by the chat window, we cant do anything with it ourselves
    private bool InternalLock = false;  //If the cursor is currently unlocked by the player to display the game menu, chat window cannot be activated

    public bool IsCursorLocked() { return CursorLocked; }
    public bool IsInternalLocked() { return InternalLock; }

    public void DisableChatCursorLock()
    {
        CursorLocked = false;
        CursorChat = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CharacterController.PreviousState = CharacterController.ControllerState;
        CharacterController.ControllerState = PlayerControllerState.Disabled;
    }

    public void EnableChatCursorLock()
    {
        CursorLocked = true;
        CursorChat = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CharacterController.ControllerState = CharacterController.PreviousState;
    }

    private void DisableCursorInternalLock()
    {
        //Cant do anything here while the cursor is being controlled by the chat input field
        if (CursorChat)
            return;

        MenuStateManager.GetMenuComponents("UI").GetComponent<MenuComponentObjects>().GetComponentObject("Quit Button").SetActive(true);
        MenuStateManager.GetMenuComponents("UI").GetComponent<MenuComponentObjects>().GetComponentObject("Change Character Button").SetActive(true);
        MenuStateManager.GetMenuComponents("UI").GetComponent<MenuComponentObjects>().GetComponentObject("Logout Account Button").SetActive(true);
        CursorLocked = false;
        InternalLock = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //CharacterController.PreviousState = CharacterController.ControllerState;
        //CharacterController.ControllerState = PlayerControllerState.Disabled;
    }

    private void EnableCursorInternalLock()
    {
        //Cant do anything here while the cursor is being controlled by the chat input field
        if (CursorChat)
            return;

        MenuStateManager.GetMenuComponents("UI").GetComponent<MenuComponentObjects>().GetComponentObject("Quit Button").SetActive(false);
        MenuStateManager.GetMenuComponents("UI").GetComponent<MenuComponentObjects>().GetComponentObject("Change Character Button").SetActive(false);
        MenuStateManager.GetMenuComponents("UI").GetComponent<MenuComponentObjects>().GetComponentObject("Logout Account Button").SetActive(false);
        CursorLocked = true;
        InternalLock = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CharacterController.ControllerState = CharacterController.PreviousState;
    }

    private void Start()
    {
        //Store our current rotation values
        Vector3 Angles = transform.eulerAngles;
        CurrentX = Angles.x;
        CurrentY = Angles.y;
        //Lock the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //Pressing escape while the cursor is locked disables the cursor lock and displays the game menu
        if (CursorLocked && Input.GetKeyDown(KeyCode.Escape))
            DisableCursorInternalLock();
        //Pressing escape while the cursor is unlocked enabled the cursor lock and hides the game menu
        else if (!CursorLocked && Input.GetKeyDown(KeyCode.Escape))
            EnableCursorInternalLock();
    }

    //Camera controls should always be done inside the LateUpdate method
    private void LateUpdate()
    {
        switch (CharacterController.ControllerState)
        {
            case (PlayerControllerState.FirstPersonMode):
                FirstPersonMode();
                break;
            case (PlayerControllerState.ThirdPersonMode):
                ThirdPersonMode();
                break;
            case (PlayerControllerState.Disabled):
                DisabledMode();
                break;
        }
    }

    //Activated when zooming the camera all the way in during third person mode
    private void StartFirstPersonMode()
    {
        GetComponent<Camera>().fieldOfView = FirstPersonFOV;
        CharacterController.ControllerState = PlayerControllerState.FirstPersonMode;
        transform.position = FirstPersonPositionAnchor.transform.position;
        transform.LookAt(FirstPersonDirectionAnchor.transform);
        transform.parent = FirstPersonPositionAnchor.transform;//CharacterController.transform;
        Vector3 Angles = transform.eulerAngles;
        CurrentX = Angles.x;
        CurrentY = Angles.y;
    }

    //Allows the player full control of their FPS camera
    private void FirstPersonMode()
    {
        //Moving the cursor up and down rotates the camera on the X axis to look up and down
        transform.Rotate(-Vector3.right * Input.GetAxis("Mouse Y") * FirstPersonMouseYSpeed * Time.deltaTime);
        //Moving the cursor left and right rotates the player character on its Y axis to turn left and right
        PlayerTransform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * FirstPersonMouseXSpeed * Time.deltaTime);
        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
            StartThirdPersonMode();
    }

    //Activated when zooming the camera out during first person mode
    private void StartThirdPersonMode()
    {
        GetComponent<Camera>().fieldOfView = ThirdPersonFOV;
        CharacterController.ControllerState = PlayerControllerState.ThirdPersonMode;
        transform.parent = null;
        Vector3 Angles = transform.eulerAngles;
        CurrentX = Angles.x;
        CurrentY = Angles.y;
    }

    //Allows the player full control of their 3rd person camera
    private void ThirdPersonMode()
    {
        //Moving the mouse cursor around will rotate the camera around the player
        CurrentX += Input.GetAxis("Mouse X") * ThirdPersonMouseXSpeed * CurrentCameraDistance * 0.02f;
        CurrentY -= Input.GetAxis("Mouse Y") * ThirdPersonMouseYSpeed * 0.02f;
        CurrentY = ClampAngle(CurrentY, YMinimum, YMaximum);
        //Scrolling the mouse wheel adjusts the distance between the camera and the player, lets you zoom in and out
        float CameraZoomAdjust = Input.GetAxis("Mouse ScrollWheel");
        float DesiredCameraDistance = CurrentCameraDistance - CameraZoomAdjust * 5;
        //Zooming the camera in closer than the minimum allowed distance activates first person mode
        if (DesiredCameraDistance < MinimumCameraDistance)
        {
            StartFirstPersonMode();
            return;
        }
        CurrentCameraDistance = Mathf.Clamp(CurrentCameraDistance - CameraZoomAdjust * 5, MinimumCameraDistance, MaximumCameraDistance);

        //Find a new target position and rotation for the player camera based on all of this input so far
        Quaternion NewRotation = Quaternion.Euler(CurrentY, CurrentX, 0f);
        Vector3 NewPosition = NewRotation * new Vector3(0f, 0f, -CurrentCameraDistance) + ThirdPersonCameraTarget.transform.position;
        //Move the camera to match these new values
        transform.position = NewPosition;
        transform.rotation = NewRotation;
    }

    //Applies movement to the third person camera as if no user input has been recieved
    private void ThirdPersonDisabled()
    {
        CurrentX += 0f * ThirdPersonMouseXSpeed * CurrentCameraDistance * 0.02f;
        CurrentY -= 0f * ThirdPersonMouseYSpeed * 0.02f;
        CurrentY = ClampAngle(CurrentY, YMinimum, YMaximum);
        Quaternion NewRotation = Quaternion.Euler(CurrentY, CurrentX, 0f);
        Vector3 NewPosition = NewRotation * new Vector3(0f, 0f, -CurrentCameraDistance) + ThirdPersonCameraTarget.transform.position;
        transform.position = NewPosition;
        transform.rotation = NewRotation;
    }

    private float ClampAngle(float Angle, float Minimum, float Maximum)
    {
        if (Angle < -360)
            Angle += 360;
        if (Angle > 360)
            Angle -= 360;
        return Mathf.Clamp(Angle, Minimum, Maximum);
    }

    private void DisabledMode()
    {
        if (CharacterController.PreviousState == PlayerControllerState.ThirdPersonMode)
            ThirdPersonDisabled();
    }
}
