// ================================================================================================================================
// File:        FootStepSoundEffects.cs
// Description: Takes an array of different sound effects, trigger by the Step event in animations plays a random sound from the list
// ================================================================================================================================

using UnityEngine;

public class FootStepSoundEffects : MonoBehaviour
{
    [SerializeField] private AudioClip[] FootStepSounds;
    [SerializeField] private AudioSource Source;

    //If we were in the air last frame and on the ground now, play a footstep sound
    [SerializeField] private CharacterController Controller;
    private bool GroundedLastFrame = true;

    private void Update()
    {
        bool IsGrounded = Controller.isGrounded;
        if (IsGrounded && !GroundedLastFrame)
            Step();
        GroundedLastFrame = IsGrounded;
    }

    private void Step()
    {
        AudioClip Clip = GetRandomClip();
        Source.PlayOneShot(Clip);
    }

    private AudioClip GetRandomClip()
    {
        return FootStepSounds[Random.Range(0, FootStepSounds.Length)];
    }
}
