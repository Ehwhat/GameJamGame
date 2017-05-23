using UnityEngine;
using System.Collections;
using System;

public class KeepAreaClearObjective : BoundObjective, IActivatableObject
{

    private bool _isActive = false;
    private bool _isEnemyInside = false;
    private float _currentTimeClear;

    [SerializeField]
    public float _timeGoal = 30;
    public static string PROPERTY_TIME_GOAL = "_timeGoal";

    public Projector _timeProjector;
    public static string PROPERTY_TOME_PROJ = "_timeProjector";

    void Start()
    {
        _timeProjector.material = new Material(_timeProjector.material);
        _timeProjector.material.SetFloat("_Radius", _boundsRadius/2);

    }

    public void OnActivate(PlayerManager player)
    {
        if (!_isActive)
        {
            SetUITime(0);
            _isActive = true;
            for (int i = 0; i < 2; i++)
            {
                GameManager.SpawnSquad(transform.position);
            }
            StartCoroutine(CheckStep());
        }
    }

    public void OnDeactivate(PlayerManager player)
    {
        
    }

    public bool ActivateCheck(PlayerManager player)
    {
        return !_isActive;
    }

    public IEnumerator CheckStep()
    {
        while (_objectiveActive)
        {
            SetUITime(_currentTimeClear / _timeGoal);
            if (CheckArea("Player") && _isActive)
            {
                if (CheckArea("Enemy"))
                {
                    _isEnemyInside = true;
                }else
                {
                    _isEnemyInside = false;
                    _currentTimeClear += Time.deltaTime;
                }
                if(_currentTimeClear >= _timeGoal)
                {
                    _objectiveActive = false;
                    ObjectiveSuccess();
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void SetUITime(float i)
    {
        _timeProjector.material.SetFloat("_FillAmount", i * 360);
    }

    private bool CheckArea(string tag)
    {
        Collider[] foundColliders = new Collider[0];
        if(_boundsType == BoundsType.Box)
        {
            foundColliders = Physics.OverlapBox(transform.position, new Vector3(_boundsArea.x / 2, 10, _boundsArea.y / 2));
        }else if (_boundsType == BoundsType.Circle)
        {
            foundColliders = Physics.OverlapSphere(transform.position, _boundsRadius);
        }
        if(foundColliders.Length > 0)
        {
            foreach (Collider c in foundColliders)
            {
                if(c.tag == tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

}
