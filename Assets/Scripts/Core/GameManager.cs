using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public enum GameState
    {
        Menu,
        InGame
    }

    [System.Serializable]
    public struct WeaponPrefabs
    {
        public string _name;
        public Weapon _weapon;
    }

    [System.Serializable]
    public struct LevelInfo
    {
        public string _name;
        public int _id;
    }

    private static GameManager instance;

    public new CameraTracking camera;
    [SerializeField]
    private PlayerManager playerPrefab;
    private LevelGenerator levelGenerator;

    private PlayerMenuManager playerMenuManager;
    public WeaponPrefabs[] weaponPrefabs;
    public LevelInfo[] levels;

    public GameState currentState = GameState.Menu;
    public int amountOfPlayers = 0;

    public Color[] playerColours = new Color[4];

    public bool[] activePlayers = new bool[4];
    public PlayerManager[] currentPlayers;
    PlayerMenu.PlayerInfo[] playerInfo;

    public float gameScore;

    private PlayerSpawnPoint spawnPoint;
    private int sceneIndex = 0;

    public bool usePlayerInfo = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start() {

        activePlayers[0] = true;
        if(currentState == GameState.Menu)
        {
            playerMenuManager = Object.FindObjectOfType<PlayerMenuManager>();
        }
        else if (currentState == GameState.InGame)
        {
            
        }
    }

    // Update is called once per frame
    void Update() {
        
    }

    public static void LoadLevel(LevelGenerator levelGenerator, CameraTracking cam, int playerAmount)
    {
        instance.camera = cam;
        instance.currentState = GameState.InGame;
        if (levelGenerator.GenerateLevel())
        {
            if (instance.usePlayerInfo)
            {
                instance.CreatePlayers(instance.playerInfo);
            }else
            {
                instance.CreatePlayers(playerAmount);
            }
            instance.SpawnPlayers(playerAmount);
        }
        cam.blit.LerpCutoff(1, 0, 1.2f);
    }

    public static void EndGame()
    {
        if(instance.currentState == GameState.InGame)
        {
            instance.camera.blit.LerpCutoff(0, 1, 1.2f);
            instance.Invoke("StartMenu",1.2f);
        }
    }

    public void StartMenu()
    {
        instance.SetLevel("MENU");
        instance.Start();
    }

    public static void StartGame()
    {
        instance.playerInfo = instance.playerMenuManager.GetPlayerInfo();
        instance.SetLevel("GAME");
        instance.Start();
    }

    public static Vector3 GetPlayersCentre()
    {
        Vector3 distProduct = Vector3.zero;
        PlayerManager[] players = instance.currentPlayers;

        if (players.Length > 0)
        {
            distProduct = players[0].transform.position;

            if (players.Length > 1)
            {
                for (int i = 1; i < players.Length; i++)
                {
                    distProduct += players[i].transform.position;

                }
            }

            distProduct /= players.Length;
        }
        return distProduct;
    }

    /*public static List<Vector3> GetPathNodes()
    {
        return instance.levelGenerator.GetLevelPop().GetPathSpots();
    }*/

    

    void CreatePlayers(PlayerMenu.PlayerInfo[] info)
    {
        currentPlayers = new PlayerManager[4];
        for (int i = 0; i < 4; i++)
        {
            if (info[i]._active)
            {
                amountOfPlayers++;
                PlayerManager player = CreatePlayer((PlayerManager.PlayerIndex)i, info[i]);
                currentPlayers[i] = player;
                activePlayers[i] = true;
            }
        }
    }

    void CreatePlayers(int playerAmount)
    {
        currentPlayers = new PlayerManager[playerAmount];
        for (int i = 0; i < playerAmount; i++)
        {
            amountOfPlayers++;
            PlayerManager player = CreatePlayer((PlayerManager.PlayerIndex)i, new PlayerMenu.PlayerInfo(0, playerColours[i]));
            currentPlayers[i] = player;
            activePlayers[i] = true;
        }
    }

    void SpawnPlayers(int playerAmount)
    {
        for (int i = 0; i < playerAmount; i++)
        {
            if (activePlayers[i])
            {
                PlayerManager player = currentPlayers[i];
                spawnPoint.SpawnPlayer(player);
                instance.camera.RegisterTransform(player.transform);
                player.Start();
            }
        }
    }

    public void SetState(GameState state)
    {
        currentState = state;
    }

    public static float GetScore()
    {
        return instance.gameScore;
    }

    public static void SetScore(float score)
    {
        instance.gameScore = score;
    }

    public static void AddScore(float amount)
    {
        instance.gameScore += amount;
    }

    public void SetLevel(string level)
    {
        foreach(LevelInfo lvl in levels)
        {
            if(lvl._name == level)
            {
                SceneManager.LoadScene(lvl._id);
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(lvl._id));
                sceneIndex = lvl._id;
            }
        }
    }

    public static WeaponPrefabs[] GetWeaponPrefabs()
    {
        return instance.weaponPrefabs;
    }

    public static void SetPlayerActive(int i, bool active)
    {
        if(i > 0 && i < 4)
        {
            instance.activePlayers[i] = active;
        }
    }

    public static CameraTracking GetCamera()
    {
        return instance.camera;
    }

    public static PlayerManager CreatePlayer(PlayerManager.PlayerIndex index, PlayerMenu.PlayerInfo info)
    {
        PlayerManager newPlayer = Instantiate(instance.playerPrefab);
        newPlayer.playerColour = info._playerColour;
        newPlayer.SetWeapon(GetWeaponPrefabs()[info._playerWeapon]._weapon);
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
