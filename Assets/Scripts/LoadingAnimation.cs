using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimation : MonoBehaviour
{
    private Image Sprite;   //Image component which we will change the sprites of to create the animation
    public float FrameTimer = 0.25f;        //How often the frame changes
    private float NextFrame;    //How long until the next frame is show
    private int CurrentFrame = 1;   //The current frame of the animation
    public Sprite AnimationFrame1;  //Each frame of the animation
    public Sprite AnimationFrame2;
    public Sprite AnimationFrame3;
    public Sprite AnimationFrame4;

    private void Awake()
    {
        //Set the time until the next frame is shown to the animation speed value
        NextFrame = FrameTimer;
        //Store the reference to the image component we will be applying the new sprites to through the animation
        Sprite = GetComponent<Image>();
    }

    public void Update()
    {
        //Count down the time until the next frame needs to be shown
        NextFrame -= Time.deltaTime;
        //Check if its time now to view the next frame
        if(NextFrame <= 0.0f)
        {
            //Reset the frame timer
            NextFrame = FrameTimer;
            //Update the image with the next frame of the animation
            switch(CurrentFrame)
            {
                case 1:
                    CurrentFrame = 2;
                    Sprite.sprite = AnimationFrame2;
                    break;
                case 2:
                    CurrentFrame = 3;
                    Sprite.sprite = AnimationFrame3;
                    break;
                case 3:
                    CurrentFrame = 4;
                    Sprite.sprite = AnimationFrame4;
                    break;
                case 4:
                    CurrentFrame = 1;
                    Sprite.sprite = AnimationFrame1;
                    break;
            }
        }
    }
}