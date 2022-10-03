using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObj : MonoBehaviour
{
    PlayerObj _owner;
    Rigidbody _rgbd3D;
    float _force = 200;
    float _destroyTime = 4;

    public PlayerObj _myOwner
    {
        get { return _owner; }
    }

    public void InitSet(PlayerObj own)
    {
        _owner = own;
    }

    public int _damage
    {
        get; set;
    }


    void Awake()
    {
        _rgbd3D = GetComponent<Rigidbody>();
        _rgbd3D.AddForce(transform.forward * _force);
        Destroy(gameObject, _destroyTime);
    }


    void Update()
    {
        RaycastHit rHit;
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        int lMask = 1 << LayerMask.NameToLayer("Map");
        if(Physics.Raycast(ray, out rHit, 0.8f, lMask))
        {
            if (rHit.collider.CompareTag("Land"))
                HitProcessing(DefineDevHelper.eHittingType.LAND, rHit.point);
            if (rHit.collider.CompareTag("Wall"))
                HitProcessing(DefineDevHelper.eHittingType.WALL, rHit.point);
            if (rHit.collider.CompareTag("Obstacle"))
            {
                HitProcessing(DefineDevHelper.eHittingType.OBSTACLE_1, rHit.point);
                ObstacleObj obj = rHit.transform.gameObject.GetComponent<ObstacleObj>();
                obj.HittingMe(GetComponent<Collider>());
            }
        }
    }

    void HitProcessing(DefineDevHelper.eHittingType type, Vector3 pos)
    {
        GameObject prefab = null;
        Quaternion dir = Quaternion.identity;
        switch (type)
        {
            case DefineDevHelper.eHittingType.LAND:
                prefab = ResPoolManager._instance.GetFXObjectsFrom(DefineDevHelper.eFxEffectKind.LandHit_Fire);
                dir = Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)); 
                // TransformDirection : 로컬 좌표축의 회전 방향을 월드 좌표축의 회전 방향으로 변환
                // LookRotation : 방향 벡터를 쿼터니언 값으로 변환
                break;
            case DefineDevHelper.eHittingType.WALL:
                prefab = ResPoolManager._instance.GetFXObjectsFrom(DefineDevHelper.eFxEffectKind.WallHit);
                dir = Quaternion.LookRotation(transform.TransformDirection(Vector3.forward));
                break;
            case DefineDevHelper.eHittingType.OBSTACLE_1:
                prefab = ResPoolManager._instance.GetFXObjectsFrom(DefineDevHelper.eFxEffectKind.ObstacleHit);
                dir = Quaternion.LookRotation(transform.TransformDirection(Vector3.forward));
                break;
            case DefineDevHelper.eHittingType.OBSTACLE_2:
                break;
            case DefineDevHelper.eHittingType.OBSTACLE_3:
                break;
            case DefineDevHelper.eHittingType.MONSTER:
                //Damage
                break;
        }
        if (prefab != null)
        {
            Destroy(Instantiate(prefab, pos, dir), 5);
        }
        else
            Debug.Log("이펙트가 없습니다.");

        
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Land"))
            HitProcessing(DefineDevHelper.eHittingType.LAND, other.ClosestPoint(transform.position));
        if(other.CompareTag("Wall"))
            HitProcessing(DefineDevHelper.eHittingType.WALL, other.ClosestPoint(transform.position));
        if(other.CompareTag("Obstacle"))
            HitProcessing(DefineDevHelper.eHittingType.OBSTACLE_1, other.ClosestPoint(transform.position));
    }
}
