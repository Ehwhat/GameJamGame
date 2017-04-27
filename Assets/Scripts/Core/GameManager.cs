using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public enum GameState
    {
        Menu,
        InGame
    }

	public enum GameMode
	{
		DestroyBase,
		KillUnits
	}

    private static GameManager instance;

    public LevelGenerator levelGenerator;
    public new CameraTracking camera;
    public PlayerManager playerPrefab;

    public GameState currentState = GameState.InGame;
    [Range(1,4)]
    public int amountOfPlayers = 1;

    public Color[] playerColours = new Color[4];

    private PlayerManager[] currentPlayers = new PlayerManager[4];
    private PlayerSpawnPoint spawnPoint;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start() {
        if (currentState == GameState.InGame)
        {
            if (levelGenerator.GenerateLevel())
            {
                SpawnPlayers((PlayerManager.PlayerIndex)amountOfPlayers - 1);
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }

    void SpawnPlayers(PlayerManager.PlayerIndex amount)
    {
        int realAmount = ((int)amount) + 1;
        for (int i = 0; i < realAmount; i++)
        {
            PlayerManager player = CreatePlayer((PlayerManager.PlayerIndex)i);
            spawnPoint.SpawnPlayer(player);
        }
    }

    public static PlayerManager CreatePlayer(PlayerManager.PlayerIndex index)
    {
        PlayerManager newPlayer = Instantiate(instance.playerPrefab);
        newPlayer.playerColour = instance.playerColours[(int)(index)];
        newPlayer.playerIndex = index;
        newPlayer.name = "Player " + (int)(index+1);
        instance.camera.RegisterTransform(newPlayer.transform);
        newPlayer.Start();
        if ((int)(index + 1) == 1)
        {
            newPlayer.gameObject.transform.FindChild("Mesh/Capsule").GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            newPlayer.gameObject.transform.FindChild("Mesh/Capsule").GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        return newPlayer;
    }


    public static void SetSpawnPoint(PlayerSpawnPoint playerSpawn)
    {
        instance.spawnPoint = playerSpawn;
    }

}
