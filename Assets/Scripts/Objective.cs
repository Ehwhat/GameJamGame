using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour {

    public GameObject baseStructurePrefab;
    public GameObject[] enemies;

    public bool complete = false;
	// Use this for initialization
	void Start () {
        Instantiate(baseStructurePrefab, transform);
	}
	
	// Update is called once per frame
	void Update ()
    {
	   
	}
}
