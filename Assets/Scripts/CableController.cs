using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableController : MonoBehaviour
{
    public Transform CableSlot;
    public Transform Cable;

    private bool _isActive;
    private Tower _currentTower;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f)), out hit, 3f,
                (1 << LayerMask.NameToLayer("Tower"))))
            {
                if (!_isActive)
                {
                    // Initiate cable
                    Cable.gameObject.SetActive(true);
                    _currentTower = hit.transform.GetComponent<Tower>();
                    _isActive = true;
                }
                else
                {
                    // Finalize cable
                    if (hit.transform.GetComponent<Tower>() != _currentTower)
                    {
                        var succ = FindObjectOfType<Transmission>().TowersConnected(hit.transform.GetComponent<Tower>(), _currentTower);

                        if (succ)
                        {
                            Cable.gameObject.SetActive(false);
                            _isActive = false;
                            _currentTower = null;
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Cable.gameObject.SetActive(false);
            _isActive = false;
            _currentTower = null;
        }

        if (_isActive)
        {
            Cable.position = (_currentTower.transform.position + CableSlot.position) / 2f;
            Cable.localScale = new Vector3(0.2f, Vector3.Distance(_currentTower.transform.position, CableSlot.position), 0.2f);
            Cable.transform.up = (_currentTower.transform.position - CableSlot.position).normalized;
        }
    }

    public void TowerOverloaded(Tower tower)
    {
        if (tower == _currentTower)
        {
            _isActive = false;
            _currentTower = null;
            Cable.gameObject.SetActive(false);
        }
    }
}
