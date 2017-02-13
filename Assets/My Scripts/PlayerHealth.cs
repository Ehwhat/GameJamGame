using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public GameObject HealthReference;

    public void setHealthBar(float myHealth)
    {
        HealthReference.transform.localScale = new Vector3(Mathf.Clamp(myHealth, 0f, 1f), HealthReference.transform.localScale.y, HealthReference.transform.localScale.z);
    }
}
