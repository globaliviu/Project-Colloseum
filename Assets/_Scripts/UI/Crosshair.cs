using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    RectTransform rect;
    public static Crosshair main;
    public Camera gunCamera;

    private float initX, initY;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        initY = rect.sizeDelta.y;
        initX = rect.sizeDelta.x;
        main = this;
    }

    private void LateUpdate()
    {
        rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition, desiredPosition, Time.deltaTime * 50f);
    }

    Vector3 desiredPosition;
    public void SetPosition(Vector3 pos)
    {
        var screenPos = gunCamera.WorldToScreenPoint(pos);
        desiredPosition = screenPos;
        
    }

    public void ApplyScale(float scaleFactor)
    {
        rect.sizeDelta = new Vector2(initX * scaleFactor, initY * scaleFactor);
    }

    public void TurnOn(bool _on)
    {
        gameObject.SetActive(_on);
    }
}
