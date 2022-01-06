using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{   
    public AudioSource audioSource;
    public Image image;
    public Sprite play;
    public Sprite pause;
    public bool isPressed = false;

    public void Pressed()
    {
        if(isPressed == false)
        {
            audioSource.Play();
            image.sprite = pause;
            isPressed = true;
        }
        else
        {
            audioSource.Pause();
            image.sprite = play;
            isPressed = false;
        }
    }
}
