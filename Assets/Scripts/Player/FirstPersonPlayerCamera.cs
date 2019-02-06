using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonPlayerCamera : MonoBehaviour
{
    public bool FirstPersonActive = false;
    [SerializeField] private FirstPersonPlayerController FirstPersonController;
    [SerializeField] private ThirdPersonPlayerController ThirdPersonController;

    public void StartFirstPersonMode()
    {
        FirstPersonActive = true;
        FirstPersonController.StartFirstPersonMode();
        ThirdPersonController.EndThirdPersonMode();
        //Position the camera correctly
        transform.position = FirstPersonPositionObject.transform.position;
        transform.LookAt(FirstPersonForwardObject.transform);
        transform.parent = PlayerCharacterObject.transform;
    }





    [SerializeField] private GameObject FirstPersonPositionObject;
    [SerializeField] private GameObject FirstPersonForwardObject;
    [SerializeField] private GameObject PlayerCharacterObject;
    private bool MouseLocked = true;
    public float MouseLookSpeed = 15;
    public float MouseTurnSpeed = 15;

    

    public void Update()
    {
        //Do nothing while in third person mode, or when the mouse is unlocked
        if (!FirstPersonActive)
            return;

        //Pressing escape releases the mouse cursor lock
        if(MouseLocked && Input.GetKeyDown(KeyCode.Escape))
        {
            MouseLocked = false;
            Cursor.lockState = CursorLockMode.None;
        }

        //Clicking the game view locks the mouse cursor
        if(!MouseLocked && Input.GetMouseButton(0))
        {
            MouseLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Cant use mouse look while the cursor is unlocked
        if (!MouseLocked)
            return;

        //Scrolling the mouse wheel to zoom out changes to third person camera mode
        if(Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            GetComponent<ThirdPersonPlayerCamera>().StartThirdPersonMode();
            ThirdPersonController.StartThirdPersonMode();
            FirstPersonActive = false;
            transform.parent = null;
            return;
        }

        //Moving the mouse cursor up and down rotates the camera on its X axis to look up and down
        float MouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(-Vector3.right * MouseY * MouseLookSpeed * Time.deltaTime);
        //Moving the mouse cursor left and right rotates the player character on its Y axis to turn left and right
        float MouseX = Input.GetAxis("Mouse X");
        PlayerCharacterObject.transform.Rotate(Vector3.up * MouseX * MouseTurnSpeed * Time.deltaTime);
    }
}
