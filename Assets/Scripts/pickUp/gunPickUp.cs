using UnityEngine;
using System.Collections;

public class gunPickUp : MonoBehaviour
{
    public ParticleSystem particles;

    public bool respawnWeapon = true;
    public float respawnTime = 1;

    private float particleStopTime;

    public Weapon weapon;
    private bool hasGiven = false;

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
            if (!respawnWeapon)
            {
                Destroy(gameObject, particles.duration);
            }
            else
            {
                StartCoroutine(RespawnWeaponCoroutine(respawnTime));
            }

        }
    }

    IEnumerator RespawnWeaponCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hasGiven = false;
        particles.Play();
        particles.time = particleStopTime + seconds;
    }
}
