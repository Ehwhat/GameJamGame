using UnityEngine;
using System.Collections;
using XInputDotNetPure;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerInput : MonoBehaviour {

    //Create XInput variables
    bool playerIndexSet = false;
    PlayerIndex playerIndex = PlayerIndex.One;
    GamePadState state;
    GamePadState prevState;

    //Movement Related Variables
    private PlayerMotor playerMotor;
    private float speed = 500f;

    void Start () {
        playerMotor = GetComponent<PlayerMotor>();
        AssignController();
    }

    private void AssignController()
    {
        //We need a way to make sure we keep this for the duration of the game...(Might break if a controller unplugs, as the player could get a different ID on reconnect. Ill sort it later)
        for (int i = 0; i < 4; ++i)
        {
            PlayerIndex testPlayerIndex = (PlayerIndex)i;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            if (testState.IsConnected)
            {
                playerIndex = testPlayerIndex;
                playerIndexSet = true;
            }
        }
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


        //Debug.Log(state.ThumbSticks.Left.X);

        //Calculate movement velocity as a 3d vector
        float _xMov = state.ThumbSticks.Left.X;
        float _zMov = state.ThumbSticks.Left.Y;

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        //Final movement vector, adjusted for Isometric perspective.
        
        Vector3 _velocity = (_movHorizontal + _movVertical);
        _velocity = Quaternion.AngleAxis(30.0f, Vector3.up) * _velocity.normalized * speed * Time.deltaTime;

        //Apply movement
        playerMotor.velocity = _velocity;

    }
}
