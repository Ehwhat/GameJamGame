using UnityEngine;
using System.Collections;

public class LevelStart : MonoBehaviour {

    new public CameraTracking camera;
    public LevelGenerator levelGenerator;

	// Use this for initialization
	void Start () {
        GameManager.LoadLevel(levelGenerator, camera);
	}

}
