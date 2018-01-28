using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image WhiteScreen;
    public Transform TitleParent;
    public AnimationCurve FadeInCurve;
    public AnimationCurve FadeOutCurve;
    public AnimationCurve AppearCurve;

    private bool _isStarting;

    void Start()
    {
        StartCoroutine(FadeIn());
        var f = 0f;
        foreach (Transform t in TitleParent)
        {
            StartCoroutine(WaveCoroutine(t, f));
            f += 0.25f;
        }

        StartCoroutine(TitleRotateCoroutine());
    }

    private IEnumerator WaveCoroutine(Transform tr, float p)
    {
        var pos = tr.position;
        while (true)
        {
            tr.position = pos + Vector3.up * Mathf.Sin(Time.time + p) * 100;
            yield return null;
        }
    }

    private IEnumerator TitleRotateCoroutine()
    {
        var f = TitleParent.rotation.eulerAngles.z;
        while (true)
        {
            TitleParent.rotation = Quaternion.Euler(0, 0, f + Mathf.Sin(Time.time) * 10f);
            yield return null;
        }
    }

    public void PlayClicked()
    {
        if (_isStarting)
        {
            return;
        }
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        const float duration = 1f;
        for (var f = 0f; f < duration; f += Time.deltaTime)
        {
            var t = FadeInCurve.Evaluate(f / duration);
            var c = WhiteScreen.color;
            c.a = t;
            WhiteScreen.color = c;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        var aso = GetComponent<AudioSource>();
        var v = aso.volume;

        _isStarting = true;
        const float duration = 3f;
        for (var f = 0f; f < duration; f += Time.deltaTime)
        {
            var t = FadeOutCurve.Evaluate(f / duration);
            var c = WhiteScreen.color;
            c.a = t;
            WhiteScreen.color = c;

            aso.volume = Mathf.Lerp(v, 0f, t);
            yield return null;
        }
        SceneManager.LoadScene("Game");
    }
}
