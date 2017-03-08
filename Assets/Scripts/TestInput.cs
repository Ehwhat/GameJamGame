using UnityEngine;
using System.Collections;

public class TestInput : UnitManager {

    PlayerControlManager.PlayerController controller;

	// Use this for initialization
	void Start () {
        controller = PlayerControlManager.GetController(XInputDotNetPure.PlayerIndex.One);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 moveVector = Vector3.zero;
        if (controller.IsConnected())
        {
            Vector2 inputVector =  controller.GetStickVector(XboxControlStick.LeftStick);
            moveVector = new Vector3(inputVector.x, 0, inputVector.y);
        }
        MoveAlongVector(moveVector, 40*Time.deltaTime);
	}
}
