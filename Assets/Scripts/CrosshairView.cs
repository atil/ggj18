using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class CrosshairView : MonoBehaviour
{
    public RectTransform[] pieces = new RectTransform[4];

    private List<Vector2> screenVertices = new List<Vector2>();
    private Vector3[] vertices;
    private Vector3[] defaultPiecePositions = new Vector3[4];
    private Vector3[] targetPiecePositions = new Vector3[4];

    void Start()
    {

        for (int i = 0; i < 4; i++)
        {
            defaultPiecePositions[i] = pieces[i].position;
        }

        // Init target position
        for (int i = 0; i < 4; i++)
        {
            targetPiecePositions[i] = defaultPiecePositions[i];
        }
    }

    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            pieces[i].position = Vector3.Lerp(pieces[i].position, targetPiecePositions[i], Time.deltaTime * 20);
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f)), out hit,
            2f, (1 << LayerMask.NameToLayer("Tower"))))
        {
            SetCrosshairTo(hit.transform.gameObject);
        }
        else
        {
            SetCrosshairTo(null);
        }
    }

    public void SetCrosshairTo(GameObject obj)
    {
        if (obj == null)
        {
            for (int i = 0; i < 4; i++)
            {
                targetPiecePositions[i] = defaultPiecePositions[i];
            }
            return;
        }
        var vertices = obj.GetComponent<MeshFilter>().mesh.vertices;

        screenVertices.Clear();

        foreach (var v in vertices)
        {
            var worldspaceV = obj.transform.TransformPoint(v);
            screenVertices.Add(Camera.main.WorldToScreenPoint(worldspaceV));
        }

        float minY = screenVertices.Min(v => v.y);
        float maxY = screenVertices.Max(v => v.y);
        float minX = screenVertices.Min(v => v.x);
        float maxX = screenVertices.Max(v => v.x);

        targetPiecePositions[0] = new Vector2(minX, maxY);
        targetPiecePositions[1] = new Vector2(minX, minY);
        targetPiecePositions[2] = new Vector2(maxX, minY);
        targetPiecePositions[3] = new Vector2(maxX, maxY);
    }

    public void SetCrosshairVisibility(bool isActive)
    {
        foreach (var piece in pieces)
        {
            piece.GetComponent<Image>().CrossFadeAlpha(isActive ? 1f : 0f, 0.1f, true);
        }
    }
}
