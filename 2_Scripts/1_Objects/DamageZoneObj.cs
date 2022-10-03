using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZoneObj : MonoBehaviour
{
    [SerializeField] float _magnify = 1;
    StatBase _owner;

    public int _finalDamage
    {
        get; set;
    }
    public void initDataSet(StatBase own)
    {
        _owner = own;
        _finalDamage = (int)(own._finalDamage * _magnify);
    }
}
