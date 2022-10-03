using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObj : MonoBehaviour
{
    [SerializeField] Transform _posFire;
    [SerializeField] ParticleSystem _muzzleFlash;

    ItemInfo _info;
    PlayerObj _owner;


    public Transform _firePosition
    {
        get { return _posFire; }
    }

    public ParticleSystem _muzzle
    {
        get { return _muzzleFlash; }
    }

    public void InitDataSet(ItemInfo info, PlayerObj owner)
    {
        _info = info;
        _owner = owner;
    }

    //총알 생성
    // GameObject _prefabBullet;
    // public void InitDataSet(GameObject prefab, PlayerObj owner)
    // {
    //     _prefabBullet = prefab;
    //     _owner = owner;
    // }

    // public void Fire(int finishDam)
    // {
    //     GameObject go = Instantiate(_prefabBullet, _posFire.position, _posFire.rotation);
    //     BulletObj obj = go.GetComponent<BulletObj>();
    //     obj.InitSet(_owner);
    //     _muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360);
    //     _muzzleFlash.Play();
    // }

}
