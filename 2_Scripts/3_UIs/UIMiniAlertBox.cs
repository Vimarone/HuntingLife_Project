using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniAlertBox : MonoBehaviour
{
    [SerializeField] Text _alertText;


    public void SetAlert(string text)
    {
        _alertText.text = text;
    }

    public void ActivateAlert(bool set)
    {
        transform.GetChild(0).gameObject.SetActive(set);
    }
}
