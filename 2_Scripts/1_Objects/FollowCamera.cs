using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Vector3 _offset = Vector3.zero;
    [SerializeField] float _followSpeed = 5;
    [SerializeField] float _angleSpeed = 3;

    Transform _playerPos;

    // Update is called once per frame
    void Update()
    {
        if (_playerPos == null) 
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if(go != null)
                _playerPos = go.transform;
        }
        else
        {
            Quaternion look = Quaternion.LookRotation(_playerPos.forward);
            Vector3 pos = _playerPos.position + (look * Vector3.forward * _offset.z) + (Vector3.up * _offset.y);
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * _followSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, _playerPos.rotation, Time.deltaTime * _angleSpeed);
        }
    }
}
