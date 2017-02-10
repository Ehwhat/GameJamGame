using UnityEngine;
using System.Collections;

public class TestSpawner : MonoBehaviour {

    public TestObject prefab;

    float lastSpawnTime;

	void Update () {
	    if(Time.time - lastSpawnTime > .3f)
        {
            TestObject spawn = prefab.GetInstanceFromPool(transform); // Here we ask the TestObject to produce a TestObject from it's pool
            spawn.origin = transform.position;
        }
	}
}
