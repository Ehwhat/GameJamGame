using UnityEngine;
using System.Collections;

public class TestObject : PooledMonoBehaviour<TestObject> { //All Poolable Objects must inheriet from the PoolMonoBehaviour

    public Vector3 origin;
    Rigidbody rb;

    private void Awake()
    {
        Debug.Log(origin);
        rb = GetComponent<Rigidbody>();
    }

    public override void OnSpawn()
    {
        transform.position = origin;
        rb.velocity = Vector3.zero;
    }

    private void Update()
    {
        if(transform.position.y < 0)
        {
            ReturnToPool(this); // This returns the TestObject to the TestObject pool.
        }
    }

}
