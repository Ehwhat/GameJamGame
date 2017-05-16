using UnityEngine;
using System.Collections;

public class PlayerMenu : MonoBehaviour {

    public class PlayerInfo
    {
        public bool _active = false;
        public int _playerWeapon = 0;
        public Color _playerColour = Color.red;

        public PlayerInfo(int weapon, Color colour)
        {
            _playerWeapon = weapon;
            _playerColour = colour;
        }

    }

    public bool menuActive = false;

    public PlayerControlManager.PlayerController controller;
    public PlayerInfo playerInfo = new PlayerInfo(0, Color.red);
    public GameObject joinMenuHolder;
    public GameObject playerMenuHolder;

    public bool _joined;

    void Update()
    {
        if (menuActive)
        {
            if (_joined)
            {
                if (controller.GetTrigger(XboxTrigger.LeftTrigger) > 0.1f)
                {
                    PlayerLeave();
                }
            }
            else
            {
                if (controller.GetTrigger(XboxTrigger.RightTrigger) > 0.1f)
                {
                    PlayerJoin();
                }
            }
        }
    }

    public void PlayerJoin()
    {
        joinMenuHolder.SetActive(false);
        playerMenuHolder.SetActive(true);
        playerInfo._active = true;
        _joined = true;
    }

    public void PlayerLeave()
    {
        joinMenuHolder.SetActive(true);
        playerMenuHolder.SetActive(false);
        playerInfo._active = false;
        _joined = false;
    }

}
