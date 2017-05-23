using UnityEngine;
using System.Collections;

public class PlayerMenuManager : MonoBehaviour {


    public bool menuActivated = false;
    public PlayerMenu[] playerMenus;
    private PlayerControlManager.PlayerController playerOneController;

    // Use this for initialization
    void Start () {
        playerMenus[0].PlayerJoin();
        GiveMenusControllers();
        playerOneController = PlayerControlManager.GetController(XInputDotNetPure.PlayerIndex.One);
    }
	
    public void ActivateMenu()
    {
        menuActivated = true;
        for (int i = 0; i < 4; i++)
        {
            playerMenus[i].menuActive = true;
        }
    }

    public void DeactivateMenu()
    {
        menuActivated = false;
        for (int i = 0; i < 4; i++)
        {
            playerMenus[i].menuActive = false;
        }
    }

	// Update is called once per frame
	void Update () {
        if (menuActivated) {
            GiveMenusControllers();
            if (!playerMenus[0]._joined)
            {

            } else {
                if (playerOneController.GetTrigger(XboxTrigger.RightTrigger) > 0.1f)
                {
                    //Send required data to the player manager
                    GameManager.StartGame();
                }
            }
        }
	}

   public void StartGame()
    {
        GameManager.StartGame();
    }

    void GiveMenusControllers()
    {
        for (int i = 0; i < 4; i++)
        {
            playerMenus[i].controller = PlayerControlManager.GetController((XInputDotNetPure.PlayerIndex)i);
        }
    }

    public PlayerMenu.PlayerInfo[] GetPlayerInfo()
    {
        PlayerMenu.PlayerInfo[] players = new PlayerMenu.PlayerInfo[4];
        for (int i = 0; i < 4; i++)
        {
            players[i] = playerMenus[i].playerInfo;
        }
        return players;
    }

}
