using UnityEngine;
using System.Collections;

public class JayPatrolPoint : MonoBehaviour
{

    public float waitTime;
    public Transform location;

    void Start()
    {
        location = gameObject.transform;
    }

}