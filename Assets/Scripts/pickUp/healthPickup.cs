using UnityEngine;
using System.Collections;

public class healthPickup : MonoBehaviour
{
    public float healAmount = 10;

    public ParticleSystem particles;
    public TextMesh pickupText;

    public bool respawnHealth = true;
    public float respawnTime = 1;

    private bool hasHealed = false;
    private float particleStopTime;

    void OnTriggerEnter(Collider c)
    {
        GameObject enteredObject = c.transform.root.gameObject;
        if(enteredObject.tag == "Player" && ! hasHealed)
        {
            PlayerManager playerManager = enteredObject.GetComponent<PlayerManager>();
            playerManager.HealFor(healAmount);
            Debug.Log("health awarded");
            hasHealed = true;
            particles.Stop();
            particleStopTime = Time.time;
            pickupText.gameObject.SetActive(false);
            if (!respawnHealth)
            {
                Destroy(gameObject, particles.duration);
            }
            else
            {
                StartCoroutine(RespawnHealthCoroutine(respawnTime));
            }

        }
    }

    IEnumerator RespawnHealthCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hasHealed = false;
        pickupText.gameObject.SetActive(true);
        particles.Play();
        particles.time = particleStopTime + seconds;
    }


}
