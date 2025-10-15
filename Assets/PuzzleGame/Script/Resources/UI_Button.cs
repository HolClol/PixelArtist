using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UI_Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnClicked;
    public event Action OnHoldClicked;

    private Vector3 initialScale;
    private Tween tween;
    private void Start()
    {
        initialScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        tween.Kill();
        tween = transform.DOScale(initialScale * 0.9f, 0.05f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        tween = transform.DOScale(initialScale, 0.25f);
    }
}
