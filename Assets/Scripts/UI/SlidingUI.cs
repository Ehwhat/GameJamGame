using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidingUI : MonoBehaviour {

    public Text text;
    public Vector3 startingPosition;
    public float speed;
    public float delay = 3;
    public Vector3 endOffset = new Vector3(0, -140, 0);

    private Vector3 endPosition;

    static private SlidingUI instance;

    void Awake()
    {
        instance = this;

        startingPosition = transform.position;
        endPosition = startingPosition + endOffset;
        SetString("");
    }

    static IEnumerator TransitionUiElement()
    {
        float time = 0;
        while (time < instance.speed) {
            instance.transform.position = Vector3.Lerp(instance.startingPosition, instance.endPosition, time/ instance.speed);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(instance.delay);
        time = 0;
        while (time < instance.speed)
        {
            instance.transform.position = Vector3.Lerp(instance.endPosition, instance.startingPosition, time / instance.speed);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

    static private void SetString(string addstring)
    {
        instance.text.text = addstring;
    }

    static public void SendSlidingMessage(string str)
    {
        SetString(str);
        instance.StartCoroutine(TransitionUiElement());


    }
}


