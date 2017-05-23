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

    public PlayerManager[] currentPlayers;
    public PlayerMenu.PlayerInfo[] playerInfo;

    public int gameScore;

    private PlayerSpawnPoint spawnPoint;
    private SquadSpawner squadSpawn;
    private int sceneIndex = 0;

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

        playerInfo[0]._active = true;
        camera = FindObjectOfType<CameraTracking>();
        if(currentState == GameState.Menu)
        {
            playerMenuManager = Object.FindObjectOfType<PlayerMenuManager>();
        }
        else if (currentState == GameState.InGame)
        {
            instance.SpawnPlayers();
        }
    }

    // Update is called once per frame
    void Update() {
        if(currentState == GameState.InGame)
        {
            CheckPlayersDead();
            CheckControllers();
        }
    }

    void CheckPlayersDead()
    {
        if (currentPlayers != null)
        {
            for (int i = 0; i < currentPlayers.Length; i++)
            {
                if (currentPlayers[i] != null && playerInfo[i]._active && !currentPlayers[i].isDead)
                {
                    return;
                }
            }
            EndGame();
        }
    }

    void CheckControllers()
    {
        if (currentPlayers != null) {
            for (int i = 0; i < 4; i++)
            {
                PlayerControlManager.PlayerController controller = PlayerControlManager.GetController((XInputDotNetPure.PlayerIndex)i);
                if (!controller.IsConnected() && currentPlayers[i].gameObject.activeSelf)
                {
                    DespawnPlayer(i);
                }
                if (controller.IsConnected() && !currentPlayers[i].gameObject.activeSelf && controller.GetTrigger(XboxTrigger.RightTrigger) > 0.3f)
                {
                    SpawnPlayer(i);
                }
            }
        }
    }

    public static void SpawnSquad(float distance)
    {
        if(instance.squadSpawn != null)
        {
            instance.squadSpawn.SpawnSquad(distance);
        }
    }

    public static void SpawnSquad(Vector3 goToPoint)
    {
        if (instance.squadSpawn != null)
        {
            instance.squadSpawn.SpawnSquad(goToPoint);
        }
    }

    public static void SpawnSquad(float distance, NavMeshEnemySquadManager squad)
    {
        if (instance.squadSpawn != null)
        {
            instance.squadSpawn.SpawnSquad(distance, squad);
        }
    }

    public static void LoadLevel(LevelGenerator levelGenerator, SquadSpawner squad, CameraTracking cam)
    {
        instance.camera = cam;
        instance.squadSpawn = squad;
        instance.currentState = GameState.InGame;
        if (levelGenerator.GenerateLevel())
        {
            instance.CreatePlayers(instance.playerInfo);
            
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
        instance.currentState = GameState.Menu;
        Debug.Log("Menu open");
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

        if (players != null)
        {
            if (instance.amountOfPlayers > 0)
            {
                distProduct = players[0].transform.position;

                int amount = 1;

                if (instance.amountOfPlayers > 1)
                {
                    for (int i = 1; i < instance.amountOfPlayers; i++)
                    {
                        if (players[i].gameObject.activeSelf)
                        {
                            distProduct += players[i].transform.position;
                            amount++;
                        }

                    }
                }

                distProduct /= amount;
            }
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
             currentPlayers[i] = CreatePlayer((PlayerManager.PlayerIndex)i, info[i]);
        }
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            if (playerInfo[i]._active)
            {
                amountOfPlayers++;
                SpawnPlayer(i);
            }
        }
    }

    void SpawnPlayer(int playerIndex)
    {
        PlayerManager player = currentPlayers[playerIndex];
        playerInfo[playerIndex]._active = true;
        AlexUIManager.SetPlayerUIActive(playerIndex, true);
        player.SetPlayerActive(true);
        spawnPoint.SpawnPlayer(player);
        instance.camera.RegisterTransform(player.transform);
        player.Start();
    }

    void SpawnPlayerAtCentre(int playerIndex)
    {
        PlayerManager player = currentPlayers[playerIndex];
        player.transform.position = GetPlayersCentre();
        AlexUIManager.SetPlayerUIActive(playerIndex, true);
        playerInfo[playerIndex]._active = true;
        player.SetPlayerActive(true);
        instance.camera.RegisterTransform(player.transform);
        player.Start();
    }

    void DespawnPlayer(int playerIndex)
    {
        PlayerManager player = currentPlayers[playerIndex];
        AlexUIManager.SetPlayerUIActive(playerIndex, false);
        playerInfo[playerIndex]._active = false;
        player.SetPlayerActive(false);
        instance.camera.DeregisterTransform(player.transform);
    }

    public static PlayerManager CreatePlayer(PlayerManager.PlayerIndex index, PlayerMenu.PlayerInfo info)
    {
        PlayerManager newPlayer = Instantiate(instance.playerPrefab);
        newPlayer.SetPlayerActive(false);
        newPlayer.playerColour = info._playerColour;
        newPlayer.SetWeapon(GetWeaponPrefabs()[info._playerWeapon]._weapon, false);
        newPlayer.playerIndex = index;
        newPlayer.name = "Player " + (int)(index + 1);
        return newPlayer;
    }


    public void SetState(GameState state)
    {
        currentState = state;
    }

    public static int GetScore()
    {
        return instance.gameScore;
    }

    public static void SetScore(int score)
    {
        instance.gameScore = score;
    }

    public static void AddScore(int amount)
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

    public static CameraTracking GetCamera()
    {
        return instance.camera;
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
