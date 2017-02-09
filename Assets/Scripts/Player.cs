using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class Player : MonoBehaviour {

    public Renderer meshRenderer;

    bool playerIndexSet = false;
    PlayerIndex playerIndex = PlayerIndex.One;
    GamePadState state;
    GamePadState prevState;


    void Start()
    {
        AssignController();
    }

    private void AssignController()
    {
        //We need a way to make sure we keep this for the duration of the game.
        for (int i = 0; i < 4; ++i)
        {
            PlayerIndex testPlayerIndex = (PlayerIndex)i;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            if (testState.IsConnected)
            {
                Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                playerIndex = testPlayerIndex;
                playerIndexSet = true;
            }
        }
    }

    void FixedUpdate()
    {
        // SetVibration should be sent in a slower rate.
        // Set vibration according to triggers
        GamePad.SetVibration(playerIndex, state.Triggers.Left, state.Triggers.Right);
    }

    void Update ()
    {
        //Grab the current state.
        prevState = state;
        state = GamePad.GetState(playerIndex);

        //Players Controller Disconnected. Show message?
        if (!prevState.IsConnected)
        {
            Debug.Log("Controller Unplugged?");
            return; // Cancel the function
        }


        Vector3 inputVelocity;

        Debug.Log(state.ThumbSticks.Left.X);

        // Make the current object turn
        transform.rotation *= Quaternion.Euler(0.0f, state.ThumbSticks.Left.X * 25.0f * Time.deltaTime, 0.0f);
    }
}
