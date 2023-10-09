using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
    public AudioSource introClip, loopClip;

    private bool isIntroDone;

    // Start is called before the first frame update
    void Start()
    {
        introClip.Play();
        isIntroDone = false;
    }

    void FixedUpdate()
    {
        if (!isIntroDone && !introClip.isPlaying) {
            introClip.Stop();
            loopClip.Play();
            enabled = false;
        }
    }
}
