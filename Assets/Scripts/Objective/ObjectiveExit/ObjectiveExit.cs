using UnityEngine;
using System.Collections;

public class ObjectiveExit : ObjectiveAbstract {

    public delegate void OnExitCallback();

    [SerializeField]
    public ExitStand _exitStand;
    public static string PROPERTY_EXIT_STAND = "_exitStand";

    [SerializeField]
    public float _exitWait;
    public static string PROPERTY_EXIT_WAIT = "_exitWait";

    [SerializeField]
    public Projector _OuterProjector;
    public static string PROPERTY_OUTER_PROJECTOR = "_OuterProjector";

    [SerializeField]
    public Projector _InnerProjector;
    public static string PROPERTY_INNER_PROJECTOR = "_InnerProjector";

    private float _currentWaitTime;
    private OnExitCallback _exitCallback;

    // Use this for initialization
    void Start () {
        _exitStand.SetCallback(OnActivated);
        _InnerProjector.material = new Material(_InnerProjector.material);
        _OuterProjector.material = new Material(_OuterProjector.material);
	}

    public void SetCallback(OnExitCallback callback)
    {
        _exitCallback = callback;
    }

    void OnActivated()
    {
        StartCoroutine(ExitWait(_exitWait));
    }
	
	IEnumerator ExitWait(float waitTime)
    {
        _currentWaitTime = 0;
        while(_currentWaitTime < waitTime)
        {
            UpdateUI();
            _currentWaitTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        OnWaitOver();
    }

    void UpdateUI()
    {
        _InnerProjector.material.SetFloat("_FillAmount", (_currentWaitTime / _exitWait)*360);
        _OuterProjector.material.SetFloat("_FillAmount", (_currentWaitTime / _exitWait)*360);
    }

    void OnWaitOver()
    {
        _exitCallback();
    } 

}
