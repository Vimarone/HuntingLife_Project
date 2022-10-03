using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMonMiniWnd : MonoBehaviour
{
    [SerializeField] Slider _hpbar;
    [SerializeField] Text _name;

    MonsterObj mbj = null;
    float _visibleTime = 5;

    float _checkTime = 0;
    Transform _target;
    
    public float _visibleTimer
    {
        get { return _visibleTime; }
    }

    void Awake()
    {
        mbj = transform.parent.GetComponent<MonsterObj>();
    }

    void Update()
    {
        if (mbj._hitTimer > 0)
        {
            mbj._hitTimer -= Time.deltaTime;
            SetHpRate(mbj._rateHP);
            if (_target != null)
            {
                Vector3 pos = new Vector3(_target.position.x, transform.position.y, _target.position.z);        // 플레이어를 향해 UI 회전(y축 고정)
                transform.LookAt(pos);
            }
        }
        else
            Visible(false);
    }

    public void OpenWindow(string n, float rate)
    {
        _name.text = n;
        _hpbar.value = rate;
    }
    public void SetHpRate(float rate)
    {
        _hpbar.value = rate;
    }
    
    public void SetHpRate(float rate, Transform t)
    {
        _hpbar.value = rate;
        Visible(true);
        if (_target != t)
            _target = t;
    }
    
    public void Visible(bool isVisi)
    {
        gameObject.SetActive(isVisi);
    }
}
