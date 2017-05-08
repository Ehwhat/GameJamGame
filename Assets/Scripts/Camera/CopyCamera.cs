using UnityEngine;
using System.Collections;

public class CopyCamera : MonoBehaviour {

    public Camera _fromCamera;
    public Camera _toCamera;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        _toCamera.orthographicSize = _fromCamera.orthographicSize;
	}
}
