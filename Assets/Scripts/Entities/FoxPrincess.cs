using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxPrincess : ServerEntity
{
    public float WalkSpeed = 500;
    public float TurnSpeed = 300;

    private void Awake()
    {
        TargetPosition = transform.position;
        TargetRotation = transform.rotation;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, TargetPosition) > 0.01f)
        {
            //Figure out where we need to move to reach our target location
            Vector3 TargetDirection = (TargetPosition - transform.position).normalized;
            Vector3 PositionTarget = transform.position + TargetDirection * Time.deltaTime;
            //Smoothly move towards this location
            transform.position = Vector3.Lerp(transform.position, PositionTarget, WalkSpeed * Time.deltaTime);
        }
        //Rotate into the correct oritentation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, TurnSpeed * Time.deltaTime);
    }
}
