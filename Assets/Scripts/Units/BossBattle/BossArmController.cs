using UnityEngine;
using System.Collections;

public class BossArmController : MonoBehaviour {

    public GameObject armSaw;

    public float armSawSpeed;
    public float armLength;
    public float armOffset;

    public float sawHeight;


    Vector3 armStart = Vector3.zero;
    Vector3 armEnd = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        getArmPoints();

        float amount = (Mathf.Sin(Time.time*armSawSpeed)+1)/2;
        armSaw.transform.position = Vector3.Lerp(armStart, armEnd, amount) - Vector3.up*sawHeight;

    }

    void OnDrawGizmos()
    {
        getArmPoints();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(armStart, armEnd);
    }

    void getArmPoints()
    {
        armStart = transform.position + transform.right * ((-armLength / 2)+armOffset);
        armEnd = transform.position + transform.right * ((armLength / 2)+armOffset);
    }

}
