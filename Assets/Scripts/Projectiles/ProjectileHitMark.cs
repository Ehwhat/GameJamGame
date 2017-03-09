using UnityEngine;
using System.Collections;

public class ProjectileHitMark : PooledMonoBehaviour<ProjectileHitMark> {

    public void Place(float lifetime)
    {
        StartCoroutine(PlaceIn(lifetime));
    }

    IEnumerator PlaceIn(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        ReturnToPool(this);
    }

}
