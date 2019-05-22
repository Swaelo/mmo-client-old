// ================================================================================================================================
// File:        PlayerCharacterController.cs
// Description: Allows the user full control of their player character, first person and third person available
// ================================================================================================================================

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
    public float WalkSpeed = 3;
    public float RunSpeed = 6;
    public float TurnSpeed = 300;
    public float GravityForce = 0.1f;
    public float YVelocity = 0.0f;
    public float JumpHeight = 3;

    //Player Camera
    public Transform CameraTransform;
    public PlayerCameraController CameraController;

    //Other component references that will be used by this script
    public PlayerResources Resources;
    public CharacterController ControllerComponent;
    public Animator AnimatorComponent;
    public PlayerTargetLock TargetLock;

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
        //The player character and their camera are grouped together as a single prefab when spawned into the scene
        //We need to immediately unparent the camera object from the player so it can move freely on its own
        CameraTransform.parent = null;
        PreviousFramePosition = transform.position;
        PreviousUpdatePosition = transform.position;
        PreviousUpdateRotation = transform.rotation;
        TargetLock = GetComponent<PlayerTargetLock>();
    }

    private void Update()
    {
        //Execute the correct controller function depending on what mode is currently active
        switch (ControllerState)
        {
            case (PlayerControllerState.FirstPersonMode):
                FirstPersonMode();
                break;
            case (PlayerControllerState.ThirdPersonMode):
                ThirdPersonMode();
                break;
        }

        //Keep the server up to date regarding where our character is positioned in the game world
        NextPositionUpdate -= Time.deltaTime;
        if (NextPositionUpdate <= 0.0f)
        {
            //Reset the timer every 0.25 seconds
            NextPositionUpdate = PositionUpdateInterval;
            //If our position or rotation has changed since the last time we told the server then we need to tell it again
            if (transform.position != PreviousUpdatePosition || transform.rotation != PreviousUpdateRotation)
            {
                //Tell the server our new position value
                if(ConnectionManager.Instance.IsConnected)
                    PlayerManagementPacketSender.SendPlayerUpdate(transform.position, transform.rotation);
                //Now remember what we last told the server our values where
                PreviousUpdatePosition = transform.position;
                PreviousUpdateRotation = transform.rotation;
            }
        }
    }

    private void LateUpdate()
    {
        //Calculate the distance travelled this frame
        Vector2 CurrentPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 PreviousPosition = new Vector2(PreviousFramePosition.x, PreviousFramePosition.z);
        float DistanceTravelled = Vector2.Distance(CurrentPosition, PreviousPosition);
        //Store the current position for next frames distance calculation
        PreviousFramePosition = transform.position;
        //Update the animation controller values
        AnimatorComponent.SetFloat("Movement", DistanceTravelled * 100);
        AnimatorComponent.SetBool("IsGrounded", TouchingGround());
    }

    //Calculate the new movement vector to apply to the character controller based on the users input during third person mode
    private Vector3 ComputeThirdPersonMovementVector()
    {
        Vector3 MovementDirection = Vector3.zero;
        //The player is forcibly moved forwards during attacking and rolling animations
        if (IsAttacking() || IsRolling())
            MovementDirection = transform.TransformDirection(Vector3.forward);
        //Otherwise, movement direction is based on user input, behaving differently based on if the player has an enemy target locked or not
        else
        {
            //Compute movement during target lock mode
            if(TargetLock.CurrentTarget != null)
            {
                //During target lock mode, the movement directions is straight toward the current target and strafing left and right relative to that forward direction
                Vector3 Forward = TargetLock.CurrentTarget.transform.position - transform.position;
                Forward.y = 0f;
                Forward = Forward.normalized;
                Vector3 Right = new Vector3(Forward.z, 0f, -Forward.x);
                MovementDirection = Input.GetAxis("Horizontal") * Right + Input.GetAxis("Vertical") * Forward;
                MovementDirection = MovementDirection.normalized;
            }
            //Compute movement normally
            else
            {
                //During normal movement, movement directions are fowards in the direction the camera is facing and left and right stafing relative to that
                Vector3 MovementX = Vector3.Cross(transform.up, CameraTransform.forward).normalized;
                Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
                MovementDirection = Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;
            }
        }

        return MovementDirection;
    }

    //Calculate the new movement vector to apply to the character controller based on the users input during first person mode
    private Vector3 ComputeFirstPersonMovementVector()
    {
        //The forward and right vectors are relative to the FPS cameras facing direction in this mode
        Vector3 MovementX = Vector3.Cross(transform.up, transform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
        //Calculate the normal movement vector using these axis based on user input
        return Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;
    }

    //Calculate the new target rotation value to have the character lerp towards
    private Quaternion ComputeRotation(Vector3 MovementVector)
    {
        //If there is no movement being applied to the character return the current rotation so nothing changes
        if (MovementVector == Vector3.zero)
            return transform.rotation;

        //Target Rotation is facing directly towards the current enemy target lock if the player has one
        if(TargetLock.CurrentTarget != null)
        {
            Vector3 TargetPosition = TargetLock.CurrentTarget.transform.position;
            TargetPosition.y = transform.position.y;
            return Quaternion.LookRotation(TargetPosition - transform.position);
        }
        //Otherwise as normal, we want to face the player towards the direction they have been moving in
        else
        {
            Quaternion TargetRotation = Quaternion.LookRotation(MovementVector);
            Vector3 Eulers = TargetRotation.eulerAngles;
            Eulers.x = transform.rotation.x;
            TargetRotation.eulerAngles = Eulers;
            return TargetRotation;
        }
    }

    //This function controls the player character during third person mode
    private void ThirdPersonMode()
    {
        //Check if the player is able to move their character or not
        //If they are able to move and hold shift the player can dash
        Resources.Running = Input.GetKey(KeyCode.LeftShift) && CanAct();
        //Compute the players movement speed for this frame
        float CurrentMoveSpeed = GetCurrentMovementSpeed();

        //Find the current movement vector to apply to the character
        Vector3 MovementVector = ComputeThirdPersonMovementVector();

        //If the player can act and they press the spacebar they perform a dodge roll
        if (Input.GetKey(KeyCode.Space) && CanAct())
            DodgeRoll();
        //If the player can act and they press the F key, they jump
        else if (Input.GetKey(KeyCode.F) && CanAct())
            Jump();

        //Apply gravity to the YVelocity while the character is not standing
        if (!TouchingGround())
            YVelocity -= GravityForce;
        //Add the final YVelocity value to the movement vector
        MovementVector.y += YVelocity;
        //Apply this final movement vector to the player character
        ControllerComponent.Move(MovementVector * CurrentMoveSpeed * Time.deltaTime);

        //Compute a new target rotation for the player, and if any player input was detected apply that rotation to the player
        Quaternion TargetRotation = ComputeRotation(MovementVector);
        if (MovementVector.x != 0f || MovementVector.z != 0f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, TurnSpeed * Time.deltaTime);
    }

    //Checks if the player is currently in a rolling animation
    private bool IsRolling()
    {
        AnimatorStateInfo CurrentAnimation = AnimatorComponent.GetCurrentAnimatorStateInfo(0);
        return CurrentAnimation.IsName("Roll");
    }

    //Checks if the player is currently in an attack animation
    private bool IsAttacking()
    {
        AnimatorStateInfo CurrentAnimation = AnimatorComponent.GetCurrentAnimatorStateInfo(0);
        return CurrentAnimation.IsName("Attack 1") || CurrentAnimation.IsName("Attack 2");
    }

    //The player is allowed to perform special actions if they meet all of the following conditions
    //They have a positive stamina value, they are touching the ground, they arent in the middle of an attack or roll animation
    private bool CanAct()
    {
        return HasStamina() && !StaminaRegenDelayed() && !IsRolling() && !IsAttacking();
    }

    private bool HasStamina()
    {
        return Resources.CurrentStamina > 0f;
    }

    private bool StaminaRegenDelayed()
    {
        return Resources.StaminaRegenDelay > 0f;
    }

    private float GetCurrentMovementSpeed()
    {
        //Calcualte base movement speed value based on if the player is dashing or not
        float CurrentMoveSpeed = Resources.Running ? RunSpeed : WalkSpeed;
        //Players are forcibly moved forwards at half their walk speed during attack animations
        if (IsAttacking())
            CurrentMoveSpeed = WalkSpeed / 2;
        //Players are forcibly moved fowards at their run speed during the rolling animation
        if (IsRolling())
            CurrentMoveSpeed = RunSpeed;
        return CurrentMoveSpeed;
    }

    private void DodgeRoll()
    {
        Resources.CurrentStamina -= 30;
        Resources.StaminaRegenDelay = 2;
        AnimatorComponent.SetTrigger("Roll");
    }

    private void Jump()
    {
        Resources.CurrentStamina -= 25;
        Resources.StaminaRegenDelay = 2;
        YVelocity = JumpHeight;
    }

    //This function controls the player character during first person mode
    private void FirstPersonMode()
    {
        //If the player is allowed to move their character, check if they are dashing or not
        Resources.Running = Input.GetKey(KeyCode.LeftShift) && CanAct();
        //Compute their current movement speed based on whether or not they are dashing
        float CurrentMoveSpeed = Resources.Running ? RunSpeed : WalkSpeed;

        //Find the new movement vector to apply to the character
        Vector3 MovementDirection = ComputeFirstPersonMovementVector();

        //Allow the player to perform various actions while they are able to act
        if (Input.GetKey(KeyCode.Space) && CanAct())
            DodgeRoll();
        else if (Input.GetKey(KeyCode.F) && CanAct())
            Jump();

        //Apply gravity to the players YVelocity while they are in the air
        if (!TouchingGround())
            YVelocity -= GravityForce;
        //Add the final YVelocity to the movement vector then apply that vector to the character
        MovementDirection.y += YVelocity;
        ControllerComponent.Move(MovementDirection * CurrentMoveSpeed * Time.deltaTime);
    }

    //Shoots a raycast directly down from the players feet position to determine if they are standing on the ground or not
    public bool TouchingGround()
    {
        RaycastHit GroundHit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out GroundHit, 100))
        {
            float GroundDistance = Vector3.Distance(transform.position, GroundHit.point);
            return GroundDistance <= 0.5f;
        }
        return false;
    }
}
