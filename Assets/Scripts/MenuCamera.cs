using UnityEngine;
using System.Collections;

public class MenuCamera : MonoBehaviour {
 
    public float speedFactor = 0.1f;
    public float zoomFactor = 1.0f;
    public Transform currentMount;
    //public Camera cameraComp;
    void Start()
    {

    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, currentMount.position, speedFactor);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentMount.rotation, speedFactor);
        //var velocity = Vector3.Magnitude(transform.position - lastPosition);
        //cameraComp.fieldOfView = 60 + velocity * zoomFactor;
        //cameraComp.zoomFactor = 60 + velocity * zoomFactor;
    }
    public void setMount(Transform newMount)
    {
        currentMount = newMount;
    }
}
