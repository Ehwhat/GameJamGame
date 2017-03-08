using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public enum XboxDigitalButtons
{
    A,
    B,
    X,
    Y,
    RightStick,
    LeftStick,
    RightShoulder,
    LeftShoulder,
    DPadUp,
    DPadLeft,
    DPadRight,
    DPadDown,
    Start,
    Back,
    Guide
}

public enum XboxControlStick
{
    LeftStick,
    RightStick
}

public enum XboxTrigger
{
    LeftTrigger,
    RightTrigger
}

public class PlayerControlManager : MonoBehaviour {

    struct RebindableControl<ControlType>
    {
        public ControlType button;
        public bool isRedefined;

        public RebindableControl(ControlType b, bool r = false)
        {
            button = b;
            isRedefined = r;
        }

        public void Rebind(ControlType b)
        {
            button = b;
            isRedefined = true;
        }
    }

    private static Dictionary<XboxDigitalButtons, RebindableControl<XboxDigitalButtons>>[] xboxDigitalBindings = new Dictionary<XboxDigitalButtons, RebindableControl<XboxDigitalButtons>>[4] {
        new Dictionary<XboxDigitalButtons, RebindableControl<XboxDigitalButtons>>() {
            {XboxDigitalButtons.A, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.A)},
            {XboxDigitalButtons.B, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.B)},
            {XboxDigitalButtons.X, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.X)},
            {XboxDigitalButtons.Y, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Y)},
            {XboxDigitalButtons.Start, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Start)},
            {XboxDigitalButtons.Back, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Back)},
            {XboxDigitalButtons.DPadUp, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadUp)},
            {XboxDigitalButtons.DPadLeft, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadLeft)},
            {XboxDigitalButtons.DPadDown, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadDown)},
            {XboxDigitalButtons.DPadRight, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadRight)},
            {XboxDigitalButtons.LeftStick, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.LeftStick)},
            {XboxDigitalButtons.RightStick, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.RightStick)},
            {XboxDigitalButtons.LeftShoulder, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.LeftShoulder)},
            {XboxDigitalButtons.RightShoulder, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.RightShoulder)},
            {XboxDigitalButtons.Guide, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Guide)},
        },
        new Dictionary<XboxDigitalButtons, RebindableControl<XboxDigitalButtons>>() {
            {XboxDigitalButtons.A, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.A)},
            {XboxDigitalButtons.B, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.B)},
            {XboxDigitalButtons.X, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.X)},
            {XboxDigitalButtons.Y, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Y)},
            {XboxDigitalButtons.Start, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Start)},
            {XboxDigitalButtons.Back, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Back)},
            {XboxDigitalButtons.DPadUp, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadUp)},
            {XboxDigitalButtons.DPadLeft, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadLeft)},
            {XboxDigitalButtons.DPadDown, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadDown)},
            {XboxDigitalButtons.DPadRight, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadRight)},
            {XboxDigitalButtons.LeftStick, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.LeftStick)},
            {XboxDigitalButtons.RightStick, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.RightStick)},
            {XboxDigitalButtons.LeftShoulder, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.LeftShoulder)},
            {XboxDigitalButtons.RightShoulder, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.RightShoulder)},
            {XboxDigitalButtons.Guide, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Guide)},
        },
        new Dictionary<XboxDigitalButtons, RebindableControl<XboxDigitalButtons>>() {
            {XboxDigitalButtons.A, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.A)},
            {XboxDigitalButtons.B, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.B)},
            {XboxDigitalButtons.X, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.X)},
            {XboxDigitalButtons.Y, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Y)},
            {XboxDigitalButtons.Start, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Start)},
            {XboxDigitalButtons.Back, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Back)},
            {XboxDigitalButtons.DPadUp, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadUp)},
            {XboxDigitalButtons.DPadLeft, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadLeft)},
            {XboxDigitalButtons.DPadDown, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadDown)},
            {XboxDigitalButtons.DPadRight, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadRight)},
            {XboxDigitalButtons.LeftStick, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.LeftStick)},
            {XboxDigitalButtons.RightStick, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.RightStick)},
            {XboxDigitalButtons.LeftShoulder, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.LeftShoulder)},
            {XboxDigitalButtons.RightShoulder, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.RightShoulder)},
            {XboxDigitalButtons.Guide, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Guide)},
        },
        new Dictionary<XboxDigitalButtons, RebindableControl<XboxDigitalButtons>>() {
            {XboxDigitalButtons.A, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.A)},
            {XboxDigitalButtons.B, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.B)},
            {XboxDigitalButtons.X, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.X)},
            {XboxDigitalButtons.Y, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Y)},
            {XboxDigitalButtons.Start, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Start)},
            {XboxDigitalButtons.Back, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Back)},
            {XboxDigitalButtons.DPadUp, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadUp)},
            {XboxDigitalButtons.DPadLeft, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadLeft)},
            {XboxDigitalButtons.DPadDown, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadDown)},
            {XboxDigitalButtons.DPadRight, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.DPadRight)},
            {XboxDigitalButtons.LeftStick, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.LeftStick)},
            {XboxDigitalButtons.RightStick, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.RightStick)},
            {XboxDigitalButtons.LeftShoulder, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.LeftShoulder)},
            {XboxDigitalButtons.RightShoulder, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.RightShoulder)},
            {XboxDigitalButtons.Guide, new RebindableControl<XboxDigitalButtons>(XboxDigitalButtons.Guide)},
        },
    };

    static private PlayerController[] playerControllers = {
        new PlayerController(PlayerIndex.One),
        new PlayerController(PlayerIndex.Two),
        new PlayerController(PlayerIndex.Three),
        new PlayerController(PlayerIndex.Four)
    };

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        UpdateGamePadStates();
    }

    void UpdateGamePadStates()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerController playerController = playerControllers[i];
            GamePadState gamePadState = GamePad.GetState((PlayerIndex)i);
            playerController.SetStates(gamePadState, playerController.gamePadState);
        }
    }

    public static PlayerController GetController(PlayerIndex player)
    {
        return playerControllers[(int)player];
    }

    static void RebindControl(PlayerIndex player, XboxDigitalButtons buttonToRebind, XboxDigitalButtons rebindToButton) {
        xboxDigitalBindings[(int)player][buttonToRebind].Rebind(rebindToButton);
    }

    static XboxDigitalButtons RawControlToReboundControl(PlayerIndex player, XboxDigitalButtons button)
    {
        return xboxDigitalBindings[(int)player][button].button;
    }

    static ButtonState XboxControltoButtonState(GamePadState state, XboxDigitalButtons button)
    {
        switch (button)
        {
            case XboxDigitalButtons.A:
                return state.Buttons.A;
            case XboxDigitalButtons.B:
                return state.Buttons.B;
            case XboxDigitalButtons.X:
                return state.Buttons.X;
            case XboxDigitalButtons.Y:
                return state.Buttons.Y;
            case XboxDigitalButtons.Start:
                return state.Buttons.Start;
            case XboxDigitalButtons.Back:
                return state.Buttons.Back;
            case XboxDigitalButtons.Guide:
                return state.Buttons.Guide;
            case XboxDigitalButtons.LeftStick:
                return state.Buttons.LeftStick;
            case XboxDigitalButtons.RightStick:
                return state.Buttons.RightStick;
            case XboxDigitalButtons.LeftShoulder:
                return state.Buttons.LeftShoulder;
            case XboxDigitalButtons.RightShoulder:
                return state.Buttons.RightShoulder;
            case XboxDigitalButtons.DPadUp:
                return state.DPad.Up;
            case XboxDigitalButtons.DPadRight:
                return state.DPad.Right;
            case XboxDigitalButtons.DPadDown:
                return state.DPad.Down;
            case XboxDigitalButtons.DPadLeft:
                return state.DPad.Left;
        }
        return (ButtonState)2;
    }

    static GamePadThumbSticks.StickValue XboxControlSticktoStickValue(GamePadState state, XboxControlStick stick)
    {
        switch (stick)
        {
            case XboxControlStick.LeftStick:
                return state.ThumbSticks.Left;
            case XboxControlStick.RightStick:
                return state.ThumbSticks.Right;
        }
        return new GamePadThumbSticks.StickValue();
    }

    static float XboxTriggertoTriggerFloat(GamePadState state, XboxTrigger trigger)
    {
        switch (trigger)
        {
            case XboxTrigger.LeftTrigger:
                return state.Triggers.Left;
            case XboxTrigger.RightTrigger:
                return state.Triggers.Right;
        }
        return 0;
    }

    public class PlayerController
    {
        PlayerIndex playerIndex;
        public GamePadState gamePadState;
        public GamePadState prevGamePadState;

        public void SetStates(GamePadState current, GamePadState prev)
        {
            gamePadState = current;
            prevGamePadState = prev;
        }

        public bool IsConnected()
        {
            return gamePadState.IsConnected;
        }

        public bool IsPressed(XboxDigitalButtons button)
        {
            button = RawControlToReboundControl(playerIndex, button);
            return (XboxControltoButtonState(prevGamePadState, button) == ButtonState.Released && XboxControltoButtonState(gamePadState, button) == ButtonState.Pressed);
        }

        public bool IsReleased(XboxDigitalButtons button)
        {
            button = RawControlToReboundControl(playerIndex, button);
            return (XboxControltoButtonState(prevGamePadState, button) == ButtonState.Pressed && XboxControltoButtonState(gamePadState, button) == ButtonState.Released);
        }

        public bool IsHeld(XboxDigitalButtons button)
        {
            button = RawControlToReboundControl(playerIndex, button);
            return (XboxControltoButtonState(gamePadState, button) == ButtonState.Pressed);
        }

        public Vector2 GetAxis(XboxControlStick stick)
        {
            GamePadThumbSticks.StickValue stickValue = XboxControlSticktoStickValue(gamePadState,stick);
            return new Vector2(stickValue.X, stickValue.Y);
        }

        public float GetTrigger(XboxTrigger trigger)
        {
            return XboxTriggertoTriggerFloat(gamePadState, trigger);
        }

        public PlayerController(PlayerIndex idx)
        {
            playerIndex = idx;
        }

        public void AssignController(GamePad gamepad)
        {

        }

    }

}
