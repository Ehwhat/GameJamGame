using UnityEngine;
using System.Collections;

public class TankVehicle : VehicleBase {

    public EntryPoint _driverEntryPoint;
    public float _trackSpeed;
    public float _trackDistance = 5;

    private Rigidbody _vehicleRigidbody;

    void Start()
    {
        _vehicleRigidbody = GetComponent<Rigidbody>();
    }

	// Update is called once per frame
	void Update () {
        DriveUpdate();

    }

    void DriveUpdate()
    {
        //KeepUprightToGround();
        PlayerControlManager.PlayerController controller = _driverEntryPoint.GetPlayerController();
        if(controller != null)
        {
            Vector2 leftStick = controller.GetStickVector(XboxControlStick.LeftStick);
            Vector2 rightStick = controller.GetStickVector(XboxControlStick.RightStick);

            float leftSpeed = leftStick.y * _trackSpeed * Time.deltaTime;
            float rightSpeed = rightStick.y * _trackSpeed * Time.deltaTime;

            SkidSteer(leftSpeed, rightSpeed);

        }
    }

    void SkidSteer(float leftSpeed, float rightSpeed)
    {
        Vector3 direction = Vector3.zero;
        float fastTread;
        float slowTread;

        if(leftSpeed == rightSpeed)
        {
            transform.position += transform.forward * leftSpeed;
        }
        else
        {
            if (Mathf.Abs(leftSpeed) < Mathf.Abs(rightSpeed))
            {
                direction = Vector3.down;
                slowTread = leftSpeed;
                fastTread = rightSpeed;
            }
            else
            {
                direction = Vector3.up;
                fastTread = leftSpeed;
                slowTread = rightSpeed;
            }

            float radius = 0;
            if(slowTread != 0)
            {
                radius = _trackDistance / ((fastTread / slowTread) - 1);
            }

            float theta = (fastTread / (radius + _trackDistance));
            float distance = ((radius + _trackDistance / 2) * Mathf.Sin(theta / 2) * 2);

            transform.Rotate(direction*(Mathf.Rad2Deg * theta / 2));
            transform.position += transform.forward * distance;
            transform.Rotate(direction * (Mathf.Rad2Deg * theta / 2));


        }
    }

    void KeepUprightToGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + transform.up, Vector3.down, out hit))
        {
            transform.up = Quaternion.FromToRotation(Vector3.forward,hit.normal).eulerAngles;
        }
    }

}
