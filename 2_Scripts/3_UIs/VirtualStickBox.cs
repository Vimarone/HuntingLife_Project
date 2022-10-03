using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualStickBox : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] bool _virtualOn = true;
    

    Image _stickBG;
    Image _stick;
    Vector3 _inputVector;
    PlayerObj _player;
    

    public float _horizVal { get { return _inputVector.x; } }

    public float _vertcVal { get { return _inputVector.y; } }

    public bool _isVirtual { get { return _virtualOn; } }

    public void InitDataSet(PlayerObj pl)
    {
        _player = pl;
    }

    void Awake()
    {
        _stickBG = GetComponent<Image>();
        _stick = transform.GetChild(0).GetComponent<Image>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_stickBG.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
            // 누른 위치가 bg 밖이면 false, bg 안이면 true 반환 및 좌표 저장(out)
        {
            pos.x = (pos.x / _stickBG.rectTransform.sizeDelta.x);
            pos.y = (pos.y / _stickBG.rectTransform.sizeDelta.y);

            _inputVector = new Vector3(pos.x, pos.y, 0);
            // 드래그 후 바깥으로 이동 : 가능 = 1이 넘는 값을 컷팅
            _inputVector = (_inputVector.magnitude > 1) ? _inputVector.normalized : _inputVector;

            // 스틱 부분을 드래그한 위치에 맞게 그래픽적으로 움직여줌(_inputVector 기반)
            _stick.rectTransform.anchoredPosition = new Vector3(_inputVector.x * (_stickBG.rectTransform.sizeDelta.x / 3), _inputVector.y * (_stickBG.rectTransform.sizeDelta.y / 3));
        }
    }

    public void OnPointerUp(PointerEventData eventData)       // 손을 떼면 원래 자리(원점)로 복귀
    {
        _inputVector = Vector3.zero;
        _stick.rectTransform.anchoredPosition = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)       // 누르면 드래그와 같은 역할
    {
        OnDrag(eventData);
    }

    public void ClickFireButton()
    {
        if (_isVirtual)
            _player.FireStart();
    }

    public void ClickReloadButton()
    {
        if (_isVirtual)
            _player.ReloadStart();
    }
}
