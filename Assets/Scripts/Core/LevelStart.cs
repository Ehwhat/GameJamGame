using UnityEngine;
using System.Collections;

public class LevelStart : MonoBehaviour {

    new public CameraTracking camera;
    public SquadSpawner squadSpawner;
    public LevelGenerator levelGenerator;

	// Use this for initialization
	void Awake () {
        GameManager.LoadLevel(levelGenerator, squadSpawner, camera);
	}

}
