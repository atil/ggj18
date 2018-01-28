using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public AudioClip SirenClip;
    private float _duration;
    private float _timer;
    private Ui _ui;

    void Start()
    {
        _ui = FindObjectOfType<Ui>();
        var t = FindObjectOfType<Transmission>();
        var aso = FindObjectOfType<Music>().GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().name == "Game")
        {
            _duration = 75f;
            t.TransmissionDuration = 45f;
            t.TowerOverloadRate = 0.03f;
            aso.clip = Resources.Load<AudioClip>("Sfx/Music2_1");
            aso.Play();
        }
        else if (SceneManager.GetActiveScene().name == "Game3")
        {
            _duration = 140f;
            t.TransmissionDuration = 90f;
            t.TowerOverloadRate = 0.02f;
            aso.clip = Resources.Load<AudioClip>("Sfx/Music1");
            aso.Play();
        }
    }

    public void Success()
    {
        StartCoroutine(SuccessCoroutine());
    }

    private IEnumerator SuccessCoroutine()
    {
        StartCoroutine(_ui.FadeOut());
        StartCoroutine(FindObjectOfType<Sfx>().FadeOut());

        yield return new WaitForSeconds(3f);
        if (SceneManager.GetActiveScene().name == "Game")
        {
            SceneManager.LoadScene("Game3");
        }
        else if (SceneManager.GetActiveScene().name == "Game3")
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private IEnumerator GameOver()
    {
        var t = FindObjectOfType<Transmission>();
        t.enabled = false;
        t.SkyColorDefault = t.SkyColorOnPowerDown;

        var m = FindObjectOfType<Music>().GetComponent<AudioSource>();
        m.Stop();
        m.PlayOneShot(SirenClip);

        yield return StartCoroutine(_ui.FadeOutBlack());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _timer = _duration;
        }

        _timer += Time.deltaTime;
        if (_timer > _duration)
        {
            StartCoroutine(GameOver());
            enabled = false;
            return;
        }
        _ui.UpdateDuration(_timer / _duration);
    }
}
