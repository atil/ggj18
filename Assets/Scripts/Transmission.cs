using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Transmission : MonoBehaviour
{
    public Tower SourceTower;
    public Tower TargetTower;
    public float FlowProgress;
    public GameObject CablePrefab;

    private readonly List<Tuple<Tower, Tower>> _connectedTowers = new List<Tuple<Tower, Tower>>();
    private bool _isTransmissionFlowing;

    public void TowersConnected(Tower a, Tower b)
    {
        _connectedTowers.Add(new Tuple<Tower, Tower>(a, b));
        var cableGo = Instantiate(CablePrefab) as GameObject;
        cableGo.transform.position = (a.transform.position + b.transform.position) / 2f;
        cableGo.transform.up = (a.transform.position - b.transform.position).normalized;
        cableGo.transform.localScale = new Vector3(0.2f, Vector3.Distance(a.transform.position, b.transform.position), 0.2f);

        RefreshConnections();
    }

    private void RefreshConnections()
    {
        var towers = new Queue<Tower>();
        towers.Enqueue(SourceTower);

        while (towers.Count != 0)
        {
            var cur = towers.Dequeue();
            var ns = GetNeighborsOf(cur);
            if (ns.Contains(TargetTower))
            {
                _isTransmissionFlowing = true;
                return;
            }

            foreach (var t in ns)
            {
                towers.Enqueue(t);
            }
        }

        _isTransmissionFlowing = false;
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
            FlowProgress += Time.deltaTime * 0.1f;
        }
        else
        {
            FlowProgress -= Time.deltaTime * 0.1f;
        }

        FlowProgress = Mathf.Clamp(FlowProgress, 0f, 1f);
    }
}
