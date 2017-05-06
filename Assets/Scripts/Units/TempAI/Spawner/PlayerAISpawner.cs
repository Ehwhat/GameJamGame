using UnityEngine;
using System.Collections;

public class PlayerAISpawner : MonoBehaviour {

	//This is a horrible basic AI spawner just so we can get the concept build out

	public float innerRadius;
	public float outerRadius;
	new public CameraTracking camera;

	public GameObject enemyPrefab;

	float multiplier;
	Vector3 position;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		multiplier = camera.GetLastZoomResult ();
		position = camera.GetLastPosition ();
	}



	void OnDrawGizmos(){
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (position, innerRadius * multiplier);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (position, outerRadius * multiplier);
	}

	void SpawnAIUnit(){
		Vector3 unitPosition = position;
		Vector2 circle = Random.insideUnitCircle * Random.Range(innerRadius, outerRadius) * multiplier;
		unitPosition += new Vector3 (circle.x, 0, circle.y);

		Instantiate (enemyPrefab, unitPosition, Quaternion.identity);

	}

}
