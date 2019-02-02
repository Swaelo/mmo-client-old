using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerCamera : MonoBehaviour
{
    public bool ThirdPersonActive = true;
    [SerializeField] private FirstPersonPlayerController FirstPersonController;
    [SerializeField] private ThirdPersonPlayerController ThirdPersonController;

    public void StartThirdPersonMode()
    {
        ThirdPersonActive = true;
        ThirdPersonController.StartThirdPersonMode();
        FirstPersonController.EndFirstPersonMode();
    }






    public GameObject PlayerTarget;

    public float CameraDistance = 10.0f;
    public float DistanceMinimum = 3f;
    public float DistanceMaximum = 15f;
    
    public float XSpeed = 250f;
    public float YSpeed = 120f;
    public float YMinimum = -20f;
    public float YMaximum = 80f;
    private float CurrentX = 0f;
    private float CurrentY = 0f;

    private bool MouseLocked = true;

    

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        CurrentX = angles.x;
        CurrentY = angles.y;
        //Lock the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Do nothing while in first person mode
        if (!ThirdPersonActive)
            return;

        //Pressing escape while the cursor is locked releases it
        if(Input.GetKeyDown(KeyCode.Escape) && MouseLocked)
        {
            MouseLocked = false;
            Cursor.lockState = CursorLockMode.None;
        }
        
        //Clicking on the game window while the cursor is free locks it again
        if(Input.GetMouseButton(0) && !MouseLocked)
        {
            MouseLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void LateUpdate()
    {
        //Do nothing while in first person mode
        if (!ThirdPersonActive)
            return;

        //Cant control the camera when the mouse cursor has been released
        if (!MouseLocked)
            return;

        CurrentX += Input.GetAxis("Mouse X") * XSpeed * CameraDistance * 0.02f;
        CurrentY -= Input.GetAxis("Mouse Y") * YSpeed * 0.02f;
        CurrentY = ClampAngle(CurrentY, YMinimum, YMaximum);
        Quaternion NewRotation = Quaternion.Euler(CurrentY, CurrentX, 0f);
        //If the camera is zoomed in closer than the minimum distance limit, then first person mode is activated
        float CameraDistanceZoomAdjust = Input.GetAxis("Mouse ScrollWheel");
        float DesiredCameraDistance = CameraDistance - CameraDistanceZoomAdjust * 5;
        if(DesiredCameraDistance < DistanceMinimum)
        {
            GetComponent<FirstPersonPlayerCamera>().StartFirstPersonMode();
            ThirdPersonActive = false;
            return;
        }
        CameraDistance = Mathf.Clamp(CameraDistance - CameraDistanceZoomAdjust * 5, DistanceMinimum, DistanceMaximum);
        Vector3 NewPosition = NewRotation * new Vector3(0f, 0f, -CameraDistance) + PlayerTarget.transform.position;
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
}
