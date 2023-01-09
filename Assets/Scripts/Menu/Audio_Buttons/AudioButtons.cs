using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioButtons : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip_highlite;
    public AudioClip clip_pressed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Highlite_Button()
    {
    //audioSource.clip = clip_highlite;
        audioSource.PlayOneShot(clip_highlite);
    }
    public void Pressed_Button()
    {
       // audioSource.clip = clip_pressed;
        audioSource.PlayOneShot(clip_pressed);
    }
}
