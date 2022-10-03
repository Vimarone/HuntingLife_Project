using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniStatusBox : MonoBehaviour
{
    [SerializeField] Slider _hpBar;
    [SerializeField] Text _name;
    [SerializeField] Image _bulletStorage;
    [SerializeField] Image _remainBullet;
    [SerializeField] Image _bulletIcon;
    [SerializeField] Image _charIcon;
    [SerializeField] Color _normalColor;
    [SerializeField] Color _dangerColor;

    //이름 동기화
    //hp바 동기화
    //한발 쏠때마다 총알 1칸씩 감소
    //다썼을때 또 쏘면 리로딩 4초(쏴지면 안됨)
    //리로딩 4초 동안 총알 1칸씩 증가
    //z키 눌러도 리로딩

    public void InitStatusBox(string name, float hpRate)
    {
        _name.text = name;
        _hpBar.value = hpRate;
    }

    public void SetHpRate(float hpRate)
    {
        _hpBar.value = hpRate;

        if (hpRate < 0.2f)
            _charIcon.color = _dangerColor;
        else
            _charIcon.color = _normalColor;
    }

    public void SetBullet(int remain, float rate)
    {
        if (rate < 0.2f)
            _bulletIcon.color = _dangerColor;
        else
            _bulletIcon.color = _normalColor;
        _remainBullet.rectTransform.sizeDelta = new Vector2(remain * 20, 0);
    }
}
