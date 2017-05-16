using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ammo : MonoBehaviour {

    public int playerAmmo;
    public Text playerAmmoUI;

	void Update ()
    {
        if(playerAmmoUI != null)
        {
            playerAmmoUI.text = playerAmmo.ToString();
        }

        if (Input.GetKeyDown("f") && playerAmmo > 0)
        {
            playerAmmo--;
        }
    }

    public void IncreaseAmmo(int amount)
    {
        playerAmmo += amount;
    }
}
