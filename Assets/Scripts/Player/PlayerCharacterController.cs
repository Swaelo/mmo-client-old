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
    [SerializeField] private float WalkSpeed = 3;
    [SerializeField] private float RunSpeed = 6;
    [SerializeField] private float TurnSpeed = 300;
    [SerializeField] private float GravityForce = 0.1f;
    [SerializeField] private float YVelocity = 0.0f;
    [SerializeField] private float JumpHeight = 3;

    private bool CurrentlyRolling = false;

    //Player Camera
    public Transform CameraTransform;
    [SerializeField] private PlayerCameraController CameraController;

    //Other component references that will be used by this script
    [SerializeField] private PlayerResources Resources;
    [SerializeField] private CharacterController ControllerComponent;
    [SerializeField] private Animator AnimatorComponent;
    [SerializeField] private PlayerTargetLock TargetLock;

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
        PlayerInfo.PlayerObject = this.gameObject;
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
            case (PlayerControllerState.Disabled):
                DisabledMode();
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
                PacketManager.Instance.SendPlayerUpdate(transform.position, transform.rotation);
                //Now remember what we last told the server our values where
                PreviousUpdatePosition = transform.position;
                PreviousUpdateRotation = transform.rotation;
            }
        }
    }

    private void LateUpdate()
    {
        //Calculate distance travelled and send it to the animator controller
        float DistanceTravelled = Vector3.Distance(transform.position, PreviousFramePosition);
        PreviousFramePosition = transform.position;
        AnimatorComponent.SetFloat("Movement", DistanceTravelled * 100);
        AnimatorComponent.SetBool("IsGrounded", TouchingGround());
    }

    //This function controls the player character during third person mode
    private void ThirdPersonMode()
    {
        //Player may only move with a positive amount of stamina, while not waiting for regeneration delay
        AnimatorStateInfo CurrentAnimation = AnimatorComponent.GetCurrentAnimatorStateInfo(0);
        bool InRollAnimation = CurrentAnimation.IsName("Roll");
        bool InAttackAnimation = CurrentAnimation.IsName("Attack 1") || CurrentAnimation.IsName("Attack 2");
        bool CanMove = Resources.CurrentStamina > 0f && Resources.StaminaRegenDelay <= 0f && !InRollAnimation && !InAttackAnimation;

        //Increase their movement speed while dashing
        Resources.Running = Input.GetKey(KeyCode.LeftShift) && CanMove;
        float MoveSpeed = Resources.Running ? RunSpeed : WalkSpeed;

        //Calculate a new movement vector, based on having a target or not and controls being locked or not
        Vector3 MovementDirection = Vector3.zero;

        //If the player is currently within the "Attack 1" or "Attack 2" animations they are not allowed to control their character
        if (InAttackAnimation)
        {//Player is forcibly moved forwards at half their walk speed during attack animations
            MoveSpeed = WalkSpeed / 2;
            MovementDirection = transform.TransformDirection(Vector3.forward);
        }
        else if (InRollAnimation)
        {
            MoveSpeed = RunSpeed;
            MovementDirection = transform.TransformDirection(Vector3.forward);
        }
        else if (TargetLock.CurrentTarget != null)
        {//Calculate movement direction relative to our current target
            Vector3 Forward = TargetLock.CurrentTarget.transform.position - transform.position;
            Forward.y = 0f;
            Forward = Forward.normalized;
            Vector3 Right = new Vector3(Forward.z, 0f, -Forward.x);
            MovementDirection = Input.GetAxis("Horizontal") * Right + Input.GetAxis("Vertical") * Forward;
            MovementDirection = MovementDirection.normalized;
        }
        else
        {//Otherwise, movement direction is based on user input
            Vector3 MovementX = Vector3.Cross(transform.up, CameraTransform.forward).normalized;
            Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
            MovementDirection = Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;
        }

        //If the player is on the ground, not currently attacking, has stamina and presses space then they perform a roll
        bool CanRoll = Resources.CurrentStamina >= 1 && TouchingGround() && !InAttackAnimation && !InRollAnimation && !CurrentlyRolling;
        if (Input.GetKey(KeyCode.Space) && CanRoll)
        {
            l.og("Roll!");
            CurrentlyRolling = true;
            Resources.CurrentStamina -= 30f;
            Resources.StaminaRegenDelay = 2f;
            AnimatorComponent.SetTrigger("Roll");
        }
        if(CurrentlyRolling)
        {
            if (CurrentAnimation.IsName("Roll"))
                CurrentlyRolling = false;
        }

        bool CanJump = Resources.CurrentStamina >= 25f && Resources.StaminaRegenDelay <= 0.0f && ControllerComponent.isGrounded && !InAttackAnimation && !InRollAnimation;
        //Update the players Y Velocity if they are jumping / falling from gravity
        if(Input.GetKey(KeyCode.F) && CanJump)
        {
            //jumping cost 25 stamina, causing 2s regen delay
            Resources.CurrentStamina -= 25f;
            Resources.StaminaRegenDelay = 2f;
            YVelocity = JumpHeight; //set velocity to jump height when on the ground and pressing the jump key
        }
        if (!TouchingGround())
            YVelocity -= GravityForce;
        MovementDirection.y += YVelocity;
        //Now apply the movement vector to the player character, if they are able to move right now
        ControllerComponent.Move(MovementDirection * MoveSpeed * Time.deltaTime);

        //Movement has been applied, now apply the new rotation
        Quaternion TargetRotation = Quaternion.identity;
        if(TargetLock.CurrentTarget != null)
        {//If we have a target locked on, face towards that
            Vector3 TargetPosition = TargetLock.CurrentTarget.transform.position;
            TargetPosition.y = transform.position.y;
            TargetRotation = Quaternion.LookRotation(TargetPosition - transform.position);
        }
        else
        {//Otherwise, face towards the direction we are moving towards
            TargetRotation = Quaternion.LookRotation(MovementDirection);
            Vector3 Eulers = TargetRotation.eulerAngles;
            Eulers.x = transform.rotation.x;
            TargetRotation.eulerAngles = Eulers;
        }
        //Now rotate to face towards the target rotation, if movement was applied this frame
        if(MovementDirection.x != 0.0f || MovementDirection.z != 0.0f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, TurnSpeed * Time.deltaTime);
    }

    private void ThirdPersonDisabled()
    {
        //Apply movement to the player as if no input has been recieved
        Vector3 MovementX = Vector3.Cross(transform.up, CameraTransform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
        Vector3 MovementVector = 0f * MovementX + 0f * MovementY;
        if (!ControllerComponent.isGrounded)
            YVelocity -= GravityForce;
        MovementVector.y += YVelocity;
        ControllerComponent.Move(MovementVector * WalkSpeed * Time.deltaTime);
        if(MovementVector.x != 0 || MovementVector.z != 0)
        {
            //Find the rotation the player would have if they were facing directly in the the direction they are moving
            Quaternion TargetRotation = Quaternion.LookRotation(MovementVector);
            Vector3 Eulers = TargetRotation.eulerAngles;
            Eulers.x = transform.rotation.x;
            TargetRotation.eulerAngles = Eulers;
            //Now rotate slowly toward that target rotation value, so the player smoothly turns around while moving
            transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, TurnSpeed * Time.deltaTime);
        }
    }

    //This function controls the player character during first person mode
    private void FirstPersonMode()
    {
        bool CanMove = Resources.StaminaRegenDelay <= 0.0f;
        Resources.Running = Input.GetKey(KeyCode.LeftShift) && CanMove;
        float MoveSpeed = Resources.Running ? RunSpeed : WalkSpeed;
        if (!CanMove)
            FirstPersonDisabled();
        else
        {
            //Get all input from the user, what direction they want to move, how fast they want to move etc.
            float XInput = Input.GetAxis("Horizontal"); //left/right movement
            float YInput = Input.GetAxis("Vertical");   //forward/back movement
            bool CanRun = Resources.CurrentStamina > 0.0f && Resources.StaminaRegenDelay <= 0.0f;
            bool IsRunning = Input.GetKey(KeyCode.LeftShift) && CanRun;
            Resources.Running = IsRunning;
            float MovementSpeed = IsRunning ? RunSpeed : WalkSpeed;
            bool Jump = ControllerComponent.isGrounded && Input.GetKey(KeyCode.Space);  //jump

            //First person movement vectors are relative to the players current facing direction
            Vector3 MovementX = Vector3.Cross(transform.up, transform.forward).normalized;
            Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
            Vector3 MovementVector = XInput * MovementX + YInput * MovementY;
            //Adjust vertical velocity and apply it to our movement vector
            YVelocity = Jump ? JumpHeight : YVelocity;
            YVelocity -= ControllerComponent.isGrounded ? 0 : GravityForce;
            MovementVector.y += YVelocity;

            //Now apply the movement vector to the player
            ControllerComponent.Move(MovementVector * MovementSpeed * Time.deltaTime);
        }
    }

    //This function applies movement to the player as if no input has been recieved from the user
    private void FirstPersonDisabled()
    {
        Vector3 MovementX = Vector3.Cross(transform.up, transform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;
        Vector3 MovementVector = 0f * MovementX + 0f * MovementY;
        if (!ControllerComponent.isGrounded)
            YVelocity -= GravityForce;
        MovementVector.y += YVelocity;
        ControllerComponent.Move(MovementVector * WalkSpeed * Time.deltaTime);
    }

    //This function continues player character movement, but doesnt listen to any user input at all
    private void DisabledMode()
    {
        //Handle differently based on what state was previously being used to control the player character
        switch(PreviousState)
        {
            case (PlayerControllerState.FirstPersonMode):
                FirstPersonDisabled();
                break;
            case (PlayerControllerState.ThirdPersonMode):
                ThirdPersonDisabled();
                break;
        }
    }

    //Shoots a raycast directly down from the players feet position to determine if they are standing on the ground or not
    public bool TouchingGround()
    {
        RaycastHit GroundHit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out GroundHit, 100))
        {
            float GroundDistance = Vector3.Distance(transform.position, GroundHit.point);
            return GroundDistance <= 0.1f;
        }
        return false;
    }
}
