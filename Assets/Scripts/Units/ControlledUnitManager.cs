using UnityEngine;
using System.Collections;

public class ControlledUnitManager : UnitManager {

    public enum PlayerIndex
    {
        One,
        Two,
        Three,
        Four
    }

    public PlayerIndex playerIndex = PlayerIndex.One;

    public PlayerControlManager.PlayerController controller;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected void GetPlayerController(PlayerIndex index)
    {
        controller = PlayerControlManager.GetController((XInputDotNetPure.PlayerIndex)index);
    }

}
