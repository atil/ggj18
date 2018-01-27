using UnityEngine;
using System.Collections;

public class Sfx : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip Footstep1;
    public AudioClip Footstep2;
    public AudioClip Jump;
    public AudioClip Land;

    public void Play(string clipName)
    {
        if (clipName == "Jump")
        {
            AudioSource.PlayOneShot(Jump);
        }

        if (clipName == "Land")
        {
            AudioSource.PlayOneShot(Land);
        }

        if (clipName == "Footstep")
        {
            AudioSource.PlayOneShot(Random.value > 0.5f ? Footstep1 : Footstep2);
        }
    }
}
