﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackingUIElement : MonoBehaviour {

    public Camera _cameraToTrackFrom;
    public Transform _trackingTransform;
    public Vector3 _offset;

    protected RectTransform _rectTransform;
    private RectTransform _canvasRect;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasRect = _rectTransform.root.GetComponent<RectTransform>();
    }

    public void UpdateTracking()
    {
        TrackingStep();
    }

    private void TrackingStep()
    {
        if(_trackingTransform != null)
        {
            Vector3 transformScreenPosition = _cameraToTrackFrom.WorldToViewportPoint(_trackingTransform.position + _offset);
            Vector3 screenPosition = new Vector3(
                (transformScreenPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f),
                (transformScreenPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f)
                );
            //transformScreenPosition.z = (2 * (transformScreenPosition.z - _cameraToTrackFrom.nearClipPlane) / (_cameraToTrackFrom.farClipPlane - _cameraToTrackFrom.nearClipPlane)) - 1f;
            _rectTransform.anchoredPosition3D = screenPosition;
        }
    }
	
}
