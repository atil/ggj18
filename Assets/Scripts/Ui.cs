using UnityEngine;
using System.Collections;

public class Ui : MonoBehaviour
{
    public RectTransform FlowProgress;

    private Transmission _transmission;

    void Start()
    {
        _transmission = FindObjectOfType<Transmission>();
    }

    void Update()
    {
        FlowProgress.localScale = new Vector3(_transmission.FlowProgress, 1, 1);
    }
}
