using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBomb : MonoBehaviour {

    public InputContext context;

    private InputContext _currentContext;

    public void OnSuccess()
    {
        Debug.Log("Bomb Defused");
    }

    public void OnFailure()
    {
        Debug.Log("Bomb Failed");
    }

	void OnMouseUpAsButton()
    {
        if(_currentContext != null)
        {
            _currentContext.Break();
        }
        _currentContext = InputContextManager.CreateNewRandomInputContext(7, false, transform, Camera.main, PlayerControlManager.GetController(0), OnSuccess, OnFailure);
    }

}
