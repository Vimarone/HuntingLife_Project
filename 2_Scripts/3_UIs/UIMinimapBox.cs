using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimapBox : MonoBehaviour
{
    [SerializeField] Text _txtMapName;

    PlayerObj _pl;
    Camera _minimapCam;
    float _limitMinSize = 10;
    float _limitMaxSize = 10;

    // 간격 안에서 사이즈 변환(플레이어가 테두리에 가도 카메라는 테두리를 벗어나지 않도록

    void Awake()
    {
        _minimapCam = GameObject.FindGameObjectWithTag("MinimapCam").GetComponent<Camera>();
        _pl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerObj>();
    }

    void Update()
    {
        _minimapCam.transform.position = new Vector3(_pl.transform.position.x, _minimapCam.transform.position.y, _pl.transform.position.z);
    }

    public void ClickZoomInButton()
    {
        if(_minimapCam.orthographicSize >= 10)
            _minimapCam.orthographicSize -= 10;
    }
    public void ClickZoomOutButton()
    {
        if(_minimapCam.orthographicSize <= 90)
            _minimapCam.orthographicSize += 10;
    }
}
