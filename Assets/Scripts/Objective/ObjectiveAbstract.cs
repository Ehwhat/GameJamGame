using UnityEngine;
using System.Collections;

public abstract class ObjectiveAbstract : MonoBehaviour {

    public delegate void ObjectiveCallback(ObjectiveAbstract objective);

    public ObjectiveCallback _onObjectiveSuccess;
    public ObjectiveCallback _onObjectiveFailure;

    [SerializeField]
    public float _objectiveSuccessScore = 100;
    public static string PROPERTY_OBJECTIVE_SUCCESS_SCORE = "_objectiveSuccessScore";

    [SerializeField]
    public float _objectiveFailureScore = -100;
    public static string PROPERTY_OBJECTIVE_FAILURE_SCORE = "_objectiveFailureScore";

    public void SetObjectiveCallbacks(ObjectiveCallback successCallback, ObjectiveCallback failureCallback)
    {
        _onObjectiveSuccess = successCallback;
        _onObjectiveFailure = failureCallback;
    }

    public void ObjectiveSuccess()
    {
        _onObjectiveSuccess(this);
    }

    public void ObjectiveFailure()
    {
        _onObjectiveFailure(this);
    }

}
