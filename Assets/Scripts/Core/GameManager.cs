using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public enum GameState
    {
        Menu,
        InGame
    }

    private static GameManager instance;

    public new CameraTracking camera;
    [SerializeField]
    private PlayerManager playerPrefab;
    private LevelGenerator levelGenerator;

    public GameState currentState = GameState.InGame;
    [Range(1,4)]
    public int amountOfPlayers = 1;

    public Color[] playerColours = new Color[4];

    public PlayerManager[] currentPlayers = new PlayerManager[4];

    private PlayerSpawnPoint spawnPoint;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
        CreatePlayers();
    }

    // Use this for initialization
    void Start() {
        if (currentState == GameState.InGame)
        {
            camera = Object.FindObjectOfType<CameraTracking>();
            levelGenerator = Object.FindObjectOfType<LevelGenerator>();
            if (levelGenerator.GenerateLevel())
            {
                SpawnPlayers(amountOfPlayers);
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }

    void CreatePlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerManager player = CreatePlayer((PlayerManager.PlayerIndex)i);
            currentPlayers[i] = player;
        }
    }

    void SpawnPlayers(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            PlayerManager player = currentPlayers[i];
            spawnPoint.SpawnPlayer(player);
            instance.camera.RegisterTransform(player.transform);
            player.Start();
        }
    }

    public static CameraTracking GetCamera()
    {
        return instance.camera;
    }

    public static PlayerManager CreatePlayer(PlayerManager.PlayerIndex index)
    {
        PlayerManager newPlayer = Instantiate(instance.playerPrefab);
        newPlayer.playerColour = instance.playerColours[(int)(index)];
        newPlayer.playerIndex = index;
        newPlayer.name = "Player " + (int)(index+1);
        return newPlayer;
    }

    public static PlayerManager GetPlayer(PlayerManager.PlayerIndex index)
    {
        return instance.currentPlayers[(int)index];
    }


    public static void SetSpawnPoint(PlayerSpawnPoint playerSpawn)
    {
        instance.spawnPoint = playerSpawn;
    }

}
