using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapOpenClose : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject _objectToOpenCloseOnClick;

    //[SerializeField] private bool _setActiveFalseOnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        _objectToOpenCloseOnClick.SetActive(true);
    }
}
