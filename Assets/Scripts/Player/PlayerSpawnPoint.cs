using UnityEngine;
using System.Collections;

public class PlayerSpawnPoint : MonoBehaviour {

    public float safeRadius = 4;

	void Awake () {
        GameManager.SetSpawnPoint(this);
	}

    public void SpawnPlayer(PlayerManager player)
    {
        Vector3 randomCircle = Random.insideUnitCircle*safeRadius;
        Vector3 randomPos = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        while (!Physics.Raycast(randomPos+Vector3.up*5, Vector3.down, 10))
        {
            randomCircle = Random.insideUnitCircle * safeRadius;
            randomPos = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
        player.transform.position = randomPos;

    }
	
}
