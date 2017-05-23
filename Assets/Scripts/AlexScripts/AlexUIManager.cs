using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlexUIManager : MonoBehaviour {

    [System.Serializable]
    public struct AlexPlayerUI
    {
        public Text weaponName;
        public Image healthBar;
        public Image ammoBar;
    }

    public AlexPlayerUI[] playerUis = new AlexPlayerUI[4];
    private static AlexPlayerUI[] staticPlayerUIs;

	// Use this for initialization
	void Start () {
        staticPlayerUIs = playerUis;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void SetPlayerHealthBar(int playerIndex, float health)
    {
        Image healthBar = GetPlayerHealthBar(playerIndex);
        healthBar.transform.localScale = new Vector3(health, 1, 1);
    }

    public static void SetPlayerAmmoBar(int playerIndex, float ammo)
    {
        Image ammoBar = GetPlayerAmmoBar(playerIndex);
        ammoBar.transform.localScale = new Vector3(ammo, 1, 1);
    }


    private static Image GetPlayerHealthBar(int playerIndex)
    {
        if(playerIndex < 4 && playerIndex > -1)
        {
            return staticPlayerUIs[playerIndex].healthBar;
        }
        return null;
    }

    private static Image GetPlayerAmmoBar(int playerIndex)
    {
        if (playerIndex < 4 && playerIndex > -1)
        {
            return staticPlayerUIs[playerIndex].ammoBar;
        }
        return null;
    }

    private static Text GetPlayerWeaponName(int playerIndex)
    {
        if(playerIndex < 4 && playerIndex > -1)
        {
            return staticPlayerUIs[playerIndex].weaponName;
        }
        return null;

    }
    public static void SetWeaponName(int playerIndex, string name)
    {
        Text text = GetPlayerWeaponName(playerIndex);
        text.text = name;
    }

}
