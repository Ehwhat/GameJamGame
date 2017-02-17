using UnityEngine;
using System.Collections;

public class PlayerManager : UnitManager {

    public GameObject aimIndicator;
    public WeaponManager weaponManager;
    public new Camera camera;

    public bool useGamepad = false;

    float inputX, inputY;
    float inputXR, inputYR;

	void Start () {
	    
	}
	
	void Update () {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        inputXR = Input.GetAxisRaw("HorizontalR");
        inputYR = Input.GetAxisRaw("VerticalR");


        Debug.Log(inputX);

        if (Mathf.Abs(inputX) > 0.5f || Mathf.Abs(inputY) > 0.5f)
        {
            float aimAngle = Mathf.Atan2(inputX, inputY) * Mathf.Rad2Deg;
            /*AimAtAngle(aimAngle);
            aimIndicator.transform.localPosition = aimDirection;
            aimIndicator.transform.rotation = Quaternion.FromToRotation(Vector3.forward,aimDirection);*/
            weaponManager.currentFiringAngle = aimAngle+isoAngleOffset;
            weaponManager.FireWeapon();
        }
        //aimIndicator.transform.forward = new Vector3(aimDirection.x,0,aimDirection.y); 
	}

    void FixedUpdate()
    {
        Vector3 currentMoveVector = new Vector3(inputX,0,inputY);
        MoveIn(currentMoveVector);
    }

}
