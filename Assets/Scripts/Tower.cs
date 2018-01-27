using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public bool IsActive;
    public bool IsOverloadable;
    public List<Tower> ConnectedTowers;
    public float OverloadProgress;
    public Renderer[] SideRenderers;
}
