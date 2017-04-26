using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour {

    public Projector circleProjector;

    void Awake()
    {
        circleProjector.material = new Material(circleProjector.material);
    }

    public void SetPlayerColour(Color colour)
    {
        circleProjector.material.SetColor("_Colour", colour);
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(Vector3.up);
    }
	
}
