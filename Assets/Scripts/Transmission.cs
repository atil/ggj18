﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Transmission : MonoBehaviour
{
    public Tower SourceTower;
    public Tower TargetTower;
    public float FlowProgress;
    public GameObject CablePrefab;
    public AnimationCurve OverloadRateToTowerCount;
    public AudioClip PowerUpClip;
    public AudioClip PowerDownClip;
    public float TransmissionDuration;
    public float TowerOverloadRate;

    public Color SkyColorOnPowerDown;
    public Color SkyColorDefault;

    private readonly List<Tuple<Tower, Tower>> _connectedTowers = new List<Tuple<Tower, Tower>>();
    private readonly Dictionary<Tuple<Tower, Tower>, GameObject> _cables = new Dictionary<Tuple<Tower, Tower>, GameObject>();
    private List<Tower> _towers;
    private bool _isTransmissionFlowing;
    private float _transmissionTimer;
    private Material _skyMaterial;
    private Color _skyCurrentColor;

    void Start()
    {
        _towers = FindObjectsOfType<Tower>().ToList();
        _skyMaterial = Camera.main.GetComponent<Skybox>().material;
        StartCoroutine(SkyColorCoroutine());
    }

    public bool TowersConnected(Tower a, Tower b)
    {
        var tuple = new Tuple<Tower, Tower>(a, b);
        var otherTuple = new Tuple<Tower, Tower>(b, a);

        if (_connectedTowers.Contains(tuple) || _connectedTowers.Contains(otherTuple))
        {
            return false;
        }

        // Brilliant amk...
        if ((a == SourceTower && b == TargetTower)
            || (b == SourceTower && a == TargetTower))
        {
            return false;
        }

        _connectedTowers.Add(tuple);
        if (!a.IsActive)
        {
            a.OverloadProgress = 0f;
        }
        a.IsActive = true;
        if (!b.IsActive)
        {
            b.OverloadProgress = 0f;
        }
        b.IsActive = true;

        AudioSource.PlayClipAtPoint(PowerUpClip, a.transform.position);
        AudioSource.PlayClipAtPoint(PowerUpClip, b.transform.position);
        a.GetComponent<AudioSource>().Play();
        b.GetComponent<AudioSource>().Play();

        var cableGo = Instantiate(CablePrefab) as GameObject;
        cableGo.transform.position = (a.transform.position + b.transform.position) / 2f;
        cableGo.transform.up = (a.transform.position - b.transform.position).normalized;
        cableGo.transform.localScale = new Vector3(0.2f, Vector3.Distance(a.transform.position, b.transform.position), 0.2f);
        _cables.Add(tuple, cableGo);

        RefreshConnections();

        return true;
    }

    private void RefreshConnections()
    {
        _isTransmissionFlowing = IsTherePathBetween(SourceTower, TargetTower);
    }

    private bool IsTherePathBetween(Tower a, Tower b)
    {
        var towers = new Queue<Tower>();
        towers.Enqueue(a);

        var visitedTowers = new HashSet<Tower>();
        while (towers.Count != 0)
        {
            var cur = towers.Dequeue();
            var ns = GetNeighborsOf(cur);
            if (ns.Contains(b))
            {
                return true;
            }

            visitedTowers.Add(cur);
            foreach (var t in ns.FindAll(x => !visitedTowers.Contains(x)))
            {
                towers.Enqueue(t);
            }
        }

        return false;
    }

    private List<Tower> GetNeighborsOf(Tower t)
    {
        var n = new List<Tower>();
        foreach (var tuple in _connectedTowers)
        {
            if (tuple.Item1 == t)
            {
                n.Add(tuple.Item2);
            }
            else if (tuple.Item2 == t)
            {
                n.Add(tuple.Item1);
            }
        }

        return n;
    }

    void Update()
    {
        if (_isTransmissionFlowing)
        {
            _transmissionTimer += Time.deltaTime;
        }
        else
        {
            _transmissionTimer -= Time.deltaTime;
        }

        _transmissionTimer = Mathf.Max(0f, _transmissionTimer);

        FlowProgress = _transmissionTimer / TransmissionDuration;
        FlowProgress = Mathf.Max(FlowProgress, 0f);
        if (FlowProgress >= 1f)
        {
            // Level done
            FindObjectOfType<Game>().Success();
            enabled = false;
            return;
        }

        foreach (var tower in _towers)
        {
            if (tower.IsOverloadable && tower.IsActive)
            {
                OverloadTower(tower);
            }
        }
    }

    private void OverloadTower(Tower tower)
    {
        var ratio = (float)_connectedTowers.Count / _towers.Count * _towers.Count - _towers.Count;
        tower.OverloadProgress += OverloadRateToTowerCount.Evaluate(ratio)
                                  * Time.deltaTime
                                  * (IsTherePathBetween(SourceTower, tower)
                                      ? TowerOverloadRate
                                      : TowerOverloadRate / 4f);

        if (tower.IsOverloadable)
        {
            foreach (var r in tower.SideRenderers)
            {
                r.material.SetFloat("_Fill", tower.OverloadProgress);
            }
        }

        if (tower.OverloadProgress >= 1f)
        {
            tower.IsActive = false;
            tower.GetComponent<AudioSource>().Stop();

            foreach (var r in tower.SideRenderers)
            {
                r.material.SetFloat("_Fill", 0f);
            }

            foreach (var tuple in _connectedTowers)
            {
                if (tuple.Item1 == tower || tuple.Item2 == tower)
                {
                    StartCoroutine(DestroyCable(_cables[tuple]));
                    _cables.Remove(tuple);
                }
            }
            _connectedTowers.RemoveAll(tuple => tuple.Item1 == tower || tuple.Item2 == tower);
            FindObjectOfType<CableController>().TowerOverloaded(tower);
            RefreshConnections();

            tower.OverloadProgress = 0f;
            AudioSource.PlayClipAtPoint(PowerDownClip, tower.transform.position);
            _skyCurrentColor = SkyColorOnPowerDown;
        }

    }

    private IEnumerator SkyColorCoroutine()
    {
        while (true)
        {
            _skyCurrentColor = Color.Lerp(_skyCurrentColor, SkyColorDefault, Time.deltaTime * 2.5f);
            _skyMaterial.SetColor("_Color2", _skyCurrentColor);
            yield return null;
        }
    }

    private IEnumerator DestroyCable(GameObject go)
    {
        go.GetComponent<Renderer>().enabled = false;
        go.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(3f);
        Destroy(go);
    }
}
