// ================================================================================================================================
// File:        LoadingAnimation.cs
// Description: Displays an animation in the game UI while waiting for something to happen, usually waiting for packets from server
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimation : MonoBehaviour
{
    private Image ImageComponent;   //UI Image Component used to render sprites on the UI
    public float AnimationSpeed = 0.25f;    //How long each frame of the animation is viewed for
    private float NextFrameTime;    //How long until the next frame of the animation is displayed

    public Sprite[] AnimationFrames;
    private int CurrentFrame = 1;
    private int FrameCount;

    private void Awake()
    {
        ImageComponent = GetComponent<Image>(); //Store reference to the Image component on this GameObject
        NextFrameTime = AnimationSpeed; //Set the timer for moving onto the next frame of the animation
        FrameCount = AnimationFrames.Length;    //Read in how many frames of animation there are
    }

    private void Update()
    {
        //Count down the timer until the next frame of the animation needs to be displayed
        NextFrameTime -= Time.deltaTime;

        //Check if its time to view the next frame of the animation
        if(NextFrameTime <= 0f)
        {
            //Reset the timer for the next frame display
            NextFrameTime = AnimationSpeed;

            //Increment the current frame counter for which frame of the animation should be displayed
            CurrentFrame++;
            if (CurrentFrame > FrameCount)
                CurrentFrame = 1;

            //Update the Image component to view the next frame of the animation
            ImageComponent.sprite = AnimationFrames[CurrentFrame - 1];
        }
    }
}