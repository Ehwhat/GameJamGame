using UnityEngine;
using System.Collections;

public class gunPickUp : MonoBehaviour
{
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
            Destroy(gameObject);
           

        }
    }
}
