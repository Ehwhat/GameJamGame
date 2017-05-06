using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropManager : MonoBehaviour {

    public class DropJob
    {
        private float timeAtStart;
        private float timeToWait;

        public DropJob(float time, float wait)
        {
            timeAtStart = time;
            timeToWait = wait;
        }

        public float GetTimePercent()
        {
            return 1-((Time.time - timeAtStart) / timeToWait);
        }

    }

    public GameObject shieldDrop;

    public void DropShield(PlayerManager player)
    {
        player.EnterDropPlace(DropShield);
    }

    private DropJob DropShield(Vector3 position)
    {
        StartCoroutine(DropWait(3, shieldDrop, position));
        return new DropJob(Time.time, 3);
    }
	
    private IEnumerator DropWait(float waitTime, GameObject drop, Vector3 position)
    {
        yield return new WaitForSeconds(waitTime);
        Drop(drop, position);
    }

    private void Drop(GameObject drop, Vector3 position)
    {
        Instantiate(drop, position, Quaternion.identity);
    }

}
