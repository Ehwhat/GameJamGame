using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuildingObject : DamageableObject {

	// Update is called once per frame
	void Update () {
	    
	}

    public override void OnDeath()
    {
        //Destroy();
    }

}
