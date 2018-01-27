using UnityEngine;
using System.Collections;

public class Sfx : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip Footstep1;
    public AudioClip Footstep2;
    public AudioClip Jump;
    public AudioClip Land;
    public AudioClip Use;
    public AudioClip Leave;

    private bool _isLandPlayable;

    public void Play(string clipName)
    {
        if (clipName == "Jump")
        {
            _isLandPlayable = true;
            AudioSource.PlayOneShot(Jump);
        }

        if (clipName == "Land")
        {
            if (_isLandPlayable)
            {
                AudioSource.PlayOneShot(Land);
                StartCoroutine(LandCooldown());
            }
        }

        if (clipName == "Footstep")
        {
            AudioSource.PlayOneShot(Random.value > 0.5f ? Footstep1 : Footstep2);
        }

        if (clipName == "Use")
        {
            AudioSource.PlayOneShot(Use);
        }

        if (clipName == "Leave")
        {
            AudioSource.PlayOneShot(Leave);
        }
    }

    private IEnumerator LandCooldown()
    {
        _isLandPlayable = false;
        yield return new WaitForSeconds(0.75f);
        _isLandPlayable = true;
    }

    public IEnumerator FadeOut()
    {
        var v = AudioSource.volume;
        for (var f = 0f; f < 3f; f += Time.deltaTime)
        {
            AudioSource.volume = Mathf.Lerp(v, 0f, f);
            yield return null;
        }
    }
}
