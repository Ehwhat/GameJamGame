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
            Vector2 inputVector = Quaternion.AngleAxis(-45,Vector3.up)*  controller.GetAxis(XboxControlStick.LeftStick);
            moveVector = new Vector3(inputVector.x, 0, inputVector.y);
        }
        MoveAlongVector(moveVector, 40*Time.deltaTime);
	}
}
