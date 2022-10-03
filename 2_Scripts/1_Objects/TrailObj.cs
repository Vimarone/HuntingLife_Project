using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailObj : MonoBehaviour
{
    float _destroyTime = 4;

    void Awake()
    {
        Destroy(gameObject, _destroyTime);
    }
}
