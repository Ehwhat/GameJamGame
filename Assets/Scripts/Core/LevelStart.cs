using UnityEngine;
using System.Collections;

public class LevelStart : MonoBehaviour {

    [Range(0,4)]
    public int playerAmount = 2;
    new public CameraTracking camera;
    public LevelGenerator levelGenerator;

	// Use this for initialization
	void Awake () {
        GameManager.LoadLevel(levelGenerator, camera, playerAmount);
	}

}
