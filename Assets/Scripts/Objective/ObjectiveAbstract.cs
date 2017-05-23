using UnityEngine;
using System.Collections;

public abstract class ObjectiveAbstract : MonoBehaviour {

    public delegate void ObjectiveCallback(ObjectiveAbstract objective);

    public ObjectiveCallback _onObjectiveSuccess;
    public ObjectiveCallback _onObjectiveFailure;

    public bool _objectiveActive = true;

    [SerializeField]
    public int _objectiveSuccessScore = 100;
    public static string PROPERTY_OBJECTIVE_SUCCESS_SCORE = "_objectiveSuccessScore";

    [SerializeField]
    public int _objectiveFailureScore = -100;
    public static string PROPERTY_OBJECTIVE_FAILURE_SCORE = "_objectiveFailureScore";

    public void AddObjectiveCallbacks(ObjectiveCallback successCallback, ObjectiveCallback failureCallback)
    {
        _onObjectiveSuccess += successCallback;
        _onObjectiveFailure += failureCallback;
    }

    public void ObjectiveSuccess()
    {
        if (_onObjectiveSuccess != null)
        {
            _onObjectiveSuccess.Invoke(this);
        }
        _objectiveActive = false;
    }

    public void ObjectiveFailure()
    {
        if (_onObjectiveFailure != null)
        {
            _onObjectiveFailure.Invoke(this);
        }
        _objectiveActive = false;
    }

}
