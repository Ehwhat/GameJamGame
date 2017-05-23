using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Just used to set the initial state of the main menu
public class MenuAutoSelect : MonoBehaviour {

	void Start () {
        GetComponent<UnityEngine.UI.Button>().Select();
	}

}
