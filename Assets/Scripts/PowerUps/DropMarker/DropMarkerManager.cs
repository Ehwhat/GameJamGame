using UnityEngine;
using System.Collections;

public class DropMarkerManager : MonoBehaviour {

    public enum DropMarkerStage
    {
        Selecting,
        Placed,
        Arrived
    }

    public Projector _dropMarkerProjector;
    private DropMarkerStage _dropMarkerStage;
    private float _dropMarkerOriginalRadius;
    private float _targetMinRadius;
    private float _targetMaxRadius;
    private float _targetSpeed;

    void Start()
    {
        _dropMarkerProjector.material = new Material(_dropMarkerProjector.material);
        _dropMarkerOriginalRadius = _dropMarkerProjector.material.GetFloat("_Radius");
        SetDropStage(DropMarkerStage.Selecting);
    }

    void Update()
    {
        ModifyDropRadius(GetSinWave01(_targetSpeed, _targetMinRadius, _targetMaxRadius));
    }

    public void SetDropStage(DropMarkerStage stage)
    {
        StopAllCoroutines();
        _dropMarkerStage = stage;
        if(_dropMarkerStage == DropMarkerStage.Selecting)
        {
            SetSinRadius(2, 1f, 1.6f);
        }else if(_dropMarkerStage == DropMarkerStage.Placed)
        {
            SetSinRadius(1, 0.5f, 0.6f);
        }
        else
        {
            SetSinRadius(1, 0, 0);
        }
    }

    void SetSinRadius(float speed, float min, float max)
    {
        _targetSpeed = speed;
        _targetMinRadius = min;
        _targetMaxRadius = max;
    }

    public void SetDropTime(float i)
    {
        _dropMarkerProjector.material.SetFloat("_FillAmount", i*360);
    }

    public void ModifyDropRadius(float i)
    {
        _dropMarkerProjector.material.SetFloat("_Radius", _dropMarkerOriginalRadius * i);
    }

    private float GetSinWave01(float speed = 1, float min = 0, float max = 1)
    {
        return min+(((Mathf.Sin(Time.time*speed) * (max - min)) *0.5f));
    }
	
}
