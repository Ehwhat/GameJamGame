using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour {

    public Projector healthProjector;
    public Projector ammoProjector;
    public Projector reloadProjector;

    void Awake()
    {
        healthProjector.material = new Material(healthProjector.material);
        ammoProjector.material = new Material(ammoProjector.material);
        reloadProjector.material = new Material(reloadProjector.material);
    }

    public void SetPlayerColour(Color colour)
    {
        healthProjector.material.SetColor("_Colour", colour);
    }

    public void SetPlayerHealth(float i)
    {
        healthProjector.material.SetFloat("_FillAmount", i * 360);
    }

    public void SetPlayerAmmo(float i)
    {
        ammoProjector.material.SetFloat("_FillAmount", i * 360);
    }

    public void SetPlayerReloadTime(float i)
    {
        reloadProjector.material.SetFloat("_FillAmount", i * 360);
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(Vector3.up);
    }
	
}
