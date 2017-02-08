using UnityEngine;
using System.Collections;

public class PlayerManager : UnitManager {

    public GameObject aimIndicator;

    float inputX, inputY;
    float inputXR, inputYR;

	void Start () {
	
	}
	
	void Update () {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        inputXR = Input.GetAxisRaw("HorizontalR");
        inputYR = Input.GetAxisRaw("VerticalR");
        aimTowardsLocal(new Vector3(inputXR,0,inputYR));
        aimIndicator.transform.localPosition = new Vector3(aimDirection.x,0,aimDirection.y); 
	}

    void FixedUpdate()
    {
        Vector3 currentMoveVector = new Vector3(inputX,0,inputY);
        MoveIn(currentMoveVector);
    }

}
