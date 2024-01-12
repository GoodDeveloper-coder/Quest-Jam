using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioSource _mouseOverButtonSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseOverButtonSound.Play();
    }
}
