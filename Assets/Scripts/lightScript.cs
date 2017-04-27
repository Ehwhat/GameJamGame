using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class lightScript : MonoBehaviour

{

    //public float totalSeconds;     // The total of seconds the flash wil last
    //public float maxIntensity;     // The maximum intensity the flash will reach
    //public Light myLight;        // Your light

    //public IEnumerator flashNow()
    //{
    //    float waitTime = totalSeconds / 2;
    //    // Get half of the seconds (One half to get brighter and one to get darker)
    //    while (myLight.intensity < maxIntensity)
    //    {
    //        myLight.intensity += Time.deltaTime / waitTime;        // Increase intensity
    //        yield return null;
    //    }
    //    while (myLight.intensity > 0)
    //    {
    //        myLight.intensity -= Time.deltaTime / waitTime;        //Decrease intensity
    //        yield return null;
    //    }
    //    yield return null;
    //}

    public Material LightOn;
    public Material LightOff;

    public float offSetTime = 1;
    public List<Transform> theLightObjects;

    void Start()
    {
        theLightObjects = new List<Transform>();
        foreach (Transform child in transform)
        {
            theLightObjects.Add(child);
        }
        theLightObjects.OrderBy(wp => wp.name);

        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while(true)
        {
            for (int i = 0; i < theLightObjects.Count; i++)
            {
                theLightObjects[i].GetComponent<Renderer>().material = LightOn;
                theLightObjects[i].GetChild(0).GetComponent<Light>().enabled = true;
                yield return new WaitForSeconds(offSetTime);
                theLightObjects[i].GetComponent<Renderer>().material = LightOff;
                theLightObjects[i].GetChild(0).GetComponent<Light>().enabled = false;
            }
        }
    }
}
