using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObj : MonoBehaviour
{
    PlayerObj player;
    int _duration = 100;
    int _defense = 2;

    public void HittingMe(Collider bullet)
    {
        BulletObj obj = bullet.transform.GetComponent<BulletObj>();
        int dam = obj._myOwner._finalDamage - _defense;
        //Debug.Log("HittingMe obj._damage : "+obj._damage);
        dam = dam < 1 ? 1 : dam;
        _duration -= dam;
        if (_duration <= 0)
        {
            GameObject go = Instantiate(ResPoolManager._instance.GetFXObjectsFrom(DefineDevHelper.eFxEffectKind.ObstacleExplode), obj.transform.localPosition, obj.transform.localRotation);
            go.transform.localScale *= 2;
            Destroy(go, 3);
            Destroy(gameObject);
        }

        Debug.Log(_duration);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            HittingMe(other);
        }
    }
}
