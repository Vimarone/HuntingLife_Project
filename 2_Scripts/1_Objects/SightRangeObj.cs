using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightRangeObj : MonoBehaviour
{
    StatBase _owner;

    public void InitSet(StatBase owner)
    {
        _owner = owner;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StatBase sb = other.GetComponent<PlayerObj>();
            if (_owner.SightOn(sb))
            {
                // 같은 놈이다....
            }
        }
    }
}
