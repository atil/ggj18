using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image WhiteScreen;
    public AnimationCurve FadeInCurve;
    public AnimationCurve FadeOutCurve;
    public AnimationCurve AppearCurve;

    private bool _isStarting;

    void Start()
    {
        StartCoroutine(FadeIn());
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
