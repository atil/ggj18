using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ui : MonoBehaviour
{
    public RectTransform FlowProgress;
    public RectTransform DurationProgress;
    public RectTransform HookFuel;
    public Image WhiteScreen;
    public Image BlackScreen;
    public AnimationCurve FadeInCurve;
    public AnimationCurve FadeOutCurve;

    private Transmission _transmission;
    private FpsController _fpsController;

    void Start()
    {
        _transmission = FindObjectOfType<Transmission>();
        _fpsController = FindObjectOfType<FpsController>();
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        FlowProgress.localScale = new Vector3(_transmission.FlowProgress, 1f, 1f);
        if (_fpsController.HookEnabled && HookFuel != null)
        {
            HookFuel.transform.localScale = new Vector3(1, _fpsController.HookFuel, 1);
        }
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
    public IEnumerator FadeOut()
    {
        const float duration = 3f;
        for (var f = 0f; f < duration; f += Time.deltaTime)
        {
            var t = FadeOutCurve.Evaluate(f / duration);
            var c = WhiteScreen.color;
            c.a = t;
            WhiteScreen.color = c;
            yield return null;
        }
    }

    public IEnumerator FadeOutBlack()
    {
        const float duration = 3f;
        for (var f = 0f; f < duration; f += Time.deltaTime)
        {
            var t = FadeOutCurve.Evaluate(f / duration);
            var c = BlackScreen.color;
            c.a = t;
            BlackScreen.color = c;
            yield return null;
        }
    }

    public void UpdateDuration(float ratio)
    {
        DurationProgress.localScale = new Vector3(ratio, 1f, 1f);
    }
}
