using UnityEngine;
using System.Collections;

public abstract class pickUp : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") //Trigger checking for collisions between player and object.
        {
            OnPickup(other.gameObject); //Pickup function called
            Destroy(gameObject); //Game Object is removed when triggered collision by player.
        }
    }

    protected abstract void OnPickup(GameObject pickupPlayer);

}
