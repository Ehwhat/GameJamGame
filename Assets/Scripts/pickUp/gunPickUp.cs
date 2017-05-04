using UnityEngine;
using System.Collections;

public class gunPickUp : MonoBehaviour
{
    public Weapon weapon;
    public ParticleSystem particles;
    private bool hasGiven = false;
    private float particleStopTime;

    void OnTriggerEnter(Collider c)
    {
        GameObject enteredObject = c.transform.root.gameObject;
        if (enteredObject.tag == "Player" && !hasGiven)
        {
            PlayerManager playerManager = enteredObject.GetComponent<PlayerManager>();
            playerManager.playerShooting.SetWeapon(weapon);
            hasGiven = true;
            particles.Stop();
            particleStopTime = Time.time;
            Destroy(gameObject);


        }
    }
}
