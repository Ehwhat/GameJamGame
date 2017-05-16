using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public GameObject HealthBarUI;

    public void setHealthBar(float myHealth)
    {
        HealthBarUI.transform.localScale = new Vector3(Mathf.Clamp(myHealth, 0f, 1f), HealthBarUI.transform.localScale.y, HealthBarUI.transform.localScale.z);
    }
}
