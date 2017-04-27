using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour {

    public GameObject baseStructurePrefab;
    public GameObject[] enemies;

    public bool complete = false;
	// Use this for initialization
	void Start () {
        GameObject o = Instantiate(baseStructurePrefab, transform) as GameObject;
        o.transform.localPosition = new Vector3(0, 0, 0);
        o.transform.localRotation = Quaternion.AngleAxis(90.0f, Vector3.up);

        StartCoroutine(CheckFinished());
	}
	
	// Update is called once per frame
	void Update ()
    {
	   
	}

    IEnumerator CheckFinished()
    {
        yield return new WaitForSeconds(2.0f);

    }
}
