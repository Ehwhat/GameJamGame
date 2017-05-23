using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjctiveCompletionManager : MonoBehaviour {

    public ObjectiveManager objectiveManager;

    RectTransform canvas;
    RectTransform button;

    Vector3 startingPosition;
    Vector3 endPosition;
    public float speed;
    public float delay = 3;

    void Start()
    {
        startingPosition = transform.position;
        endPosition = startingPosition + new Vector3(0, -140, 0);

        objectiveManager.onObjectiveSuccess += OnObjectiveSuccess;

        button = gameObject.GetComponent<RectTransform>();
        canvas = GameObject.Find("UI").GetComponent<RectTransform>();
    }

    IEnumerator SlideIn()
    {
        float time = 0;
        while (time < speed) {
            transform.position = Vector3.Lerp(startingPosition, endPosition, time/speed);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(delay);
        StartCoroutine(SlideOut());

    }

    IEnumerator SlideOut()
    {
        float time = 0;
        while (time < speed)
        {
            transform.position = Vector3.Lerp(endPosition, startingPosition, time / speed);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnObjectiveSuccess(ObjectiveAbstract obj)
    {
        StartCoroutine(SlideIn());
    }
}


