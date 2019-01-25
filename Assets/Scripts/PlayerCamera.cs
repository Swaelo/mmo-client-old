using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
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

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        CurrentX = angles.x;
        CurrentY = angles.y;
    }

    private void LateUpdate()
    {
        CurrentX += Input.GetAxis("Mouse X") * XSpeed * CameraDistance * 0.02f;
        CurrentY -= Input.GetAxis("Mouse Y") * YSpeed * 0.02f;
        CurrentY = ClampAngle(CurrentY, YMinimum, YMaximum);
        Quaternion NewRotation = Quaternion.Euler(CurrentY, CurrentX, 0f);
        CameraDistance = Mathf.Clamp(CameraDistance - Input.GetAxis("Mouse ScrollWheel") * 5, DistanceMinimum, DistanceMaximum);
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
