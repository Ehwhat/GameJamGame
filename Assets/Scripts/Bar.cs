using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bar : MonoBehaviour {

    public Transform HealthBar;
    public Transform ManaBar;
    public Transform PercentIndicator;
    public float dmg = 10;
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentMana;
    [SerializeField] private float currentAmmo;


	
	
	void Update () {
	
        if(currentHealth > 0)
        {
            //change me to dmg recieved
            currentHealth -= dmg * Time.deltaTime;
            currentMana -= dmg * Time.deltaTime;
            if (currentAmmo > 0)
            {
                //current ammo indicator
                PercentIndicator.GetComponent<Text>().text = ((int)currentAmmo).ToString();
            }
            else
            {
                //When player has no ammo
                PercentIndicator.GetComponent<Text>().text = "Empty!";
            }
        }
        else
        {
            //When player has no ammo
            PercentIndicator.GetComponent<Text>().text = "Dead!";
        }

        
        //Health bar divides by 400 so to get only a quarter bar on the UI screen
        HealthBar.GetComponent<Image>().fillAmount = currentHealth / 400;

        ManaBar.GetComponent<Image>().fillAmount = currentHealth / 400;
    }
}
