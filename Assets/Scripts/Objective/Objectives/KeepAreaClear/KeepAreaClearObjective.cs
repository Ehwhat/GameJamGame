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

    public void OnActivate(PlayerManager player)
    {
        if (!_isActive)
        {
            _isActive = true;
            StartCoroutine(CheckStep());
        }
    }

    public void OnDeactivate(PlayerManager player)
    {
        
    }

    public bool ActivateCheck(PlayerManager player)
    {
        return true;
    }

    public IEnumerator CheckStep()
    {
        while (true)
        {
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
                    ObjectiveSuccess();
                }
            }
            yield return new WaitForEndOfFrame();
        }
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
