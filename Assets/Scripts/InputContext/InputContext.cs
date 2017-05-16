using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class InputContextEvent : UnityEvent { }

[System.Serializable]
public class InputContext : TrackingUIElement {

    public enum ThumbstickPositions{
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
            None = 99
    }

    public float _sensitivy;

    private bool _active = true;

    [SerializeField]
    private bool _allowRetry = true;

    [SerializeField]
    private bool _waitForNone = true;

    [SerializeField]
    public InputContextEvent _successEvent = new InputContextEvent();

    [SerializeField]
    public InputContextEvent _failureEvent = new InputContextEvent();

    [SerializeField]
    private GameObject _directionalArrowPrefab;

    [SerializeField]
    private RectTransform _buttonHolderPrefab;

    [SerializeField]
    private PlayerControlManager.PlayerController _listenController;

    [SerializeField]
    private List<ThumbstickPositions> _commands;

    [SerializeField]
    private List<InputContextButton> _commandButtons;

    [SerializeField]
    private int _currentCommandIndex;

    private PlayerManager _owningPlayer;

    public InputContext Init(PlayerControlManager.PlayerController listenController, PlayerManager owningPlayer)
    {
        UpdateTracking();
        _listenController = listenController;
        _owningPlayer = owningPlayer;
        GenerateButtons();
        StartCoroutine(ListenForCommands());
        return this;
    }

    public InputContext InitRandom(PlayerControlManager.PlayerController listenController, PlayerManager owningPlayer, int length, bool allowResets)
    {
        UpdateTracking();
        _listenController = listenController;
        _owningPlayer = owningPlayer;
        _allowRetry = allowResets;
        GenerateRandomCommands(length);
        GenerateButtons();
        StartCoroutine(ListenForCommands());
        return this;
    }

    public InputContext InitCompareToManager(PlayerControlManager.PlayerController listenController, PlayerManager owningPlayer)
    {
        UpdateTracking();
        _listenController = listenController;
        _owningPlayer = owningPlayer;
        _commands.Clear();
        _commandButtons.Clear();
        StartCoroutine(ListenForCommandsToManager());
        return this;
    }

    void GenerateRandomCommands(int length)
    {
        _commands = new List<ThumbstickPositions>();
        for (int i = 0; i < length; i++)
        {
            _commands.Add((ThumbstickPositions)(Random.Range(0, 3)*2));
        }
    }

    void GenerateButtons()
    {
        _commandButtons = new List<InputContextButton>();
        _commandButtons.Capacity = _commands.Count;
        for (int i = 0; i < _commands.Count; i++)
        {
            _commandButtons.Add(GenerateButton(i, _commands[i], InputContextButton.ButtonState.NotPressed));
        }
        _commandButtons[_currentCommandIndex].SetState(InputContextButton.ButtonState.Current);
        UpdateTracking();
    }

    InputContextButton GenerateButton(int index, ThumbstickPositions thumbstick, InputContextButton.ButtonState state)
    {
        InputContextButton button = ((GameObject)Instantiate(_directionalArrowPrefab, _buttonHolderPrefab)).GetComponent<InputContextButton>();
        button.SetPosition(index);
        button.SetRotation(thumbstick);
        button.SetState(state);
        return button;
    }

    IEnumerator ListenForCommandsToManager()
    {
        int i = 0;
        while (_active)
        {
            ThumbstickPositions currentPosition = GetCurrentPosition();
            if(currentPosition != ThumbstickPositions.None && !_waitForNone)
            {
                _commands.Add(currentPosition);
                _commandButtons.Add(GenerateButton(i, _commands[i], InputContextButton.ButtonState.Pressed));
                if (InputContextManager.ComparePatternToManagerPattern(_commands, _owningPlayer))
                {
                    _owningPlayer.BreakContext();
                    yield break;
                }
                i++;
                _waitForNone = true;
            }else if(currentPosition == ThumbstickPositions.None)
            {
                _waitForNone = false;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ListenForCommands()
    {
        while (_active)
        {
            ThumbstickPositions currentPosition = GetCurrentPosition();
            if(currentPosition == _commands[_currentCommandIndex] && !_waitForNone)
            {
                _waitForNone = true;
                _commandButtons[_currentCommandIndex].SetState(InputContextButton.ButtonState.Pressed);
                _currentCommandIndex++;
                if (_currentCommandIndex >= _commands.Count)
                {
                    OnSuccess();
                    _active = false;
                    _owningPlayer.BreakContext();
                    yield break;
                }
                else
                {
                    _commandButtons[_currentCommandIndex].SetState(InputContextButton.ButtonState.Current);
                }
            }else if(currentPosition != ThumbstickPositions.None && !_waitForNone)
            {
                _waitForNone = true;
                OnFailure();
                if (_allowRetry)
                {
                    Reset();
                }else
                {
                    _owningPlayer.BreakContext();
                }
            }else if(currentPosition == ThumbstickPositions.None && _waitForNone)
            {
                _waitForNone = false;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnSuccess()
    {
        if (_successEvent != null)
        {
            _successEvent.Invoke();
        }
    }

    public void OnFailure()
    {
        if (_failureEvent != null)
        {
            _failureEvent.Invoke();
        }
    }

    public void Reset()
    {
        foreach(InputContextButton button in _commandButtons)
        {
            button.SetState(InputContextButton.ButtonState.NotPressed);
        }
        _commandButtons[0].SetState(InputContextButton.ButtonState.Current);
        _currentCommandIndex = 0;
    }

    public void Break()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        UpdateTracking();
    }
	
    ThumbstickPositions GetCurrentPosition()
    {
        Vector2 currentThumbstick = _listenController.GetStickVector(XboxControlStick.RightStick);
        if (currentThumbstick.magnitude > 0) {
            if (currentThumbstick.x > _sensitivy)
            {
                if (currentThumbstick.y > _sensitivy)
                {
                    return ThumbstickPositions.UpRight;
                } else if (currentThumbstick.y < -_sensitivy)
                {
                    return ThumbstickPositions.DownRight;
                }
                return ThumbstickPositions.Right;
            } else if (currentThumbstick.x < -_sensitivy)
            {
                if (currentThumbstick.y > _sensitivy)
                {
                    return ThumbstickPositions.UpLeft;
                }
                else if (currentThumbstick.y < -_sensitivy)
                {
                    return ThumbstickPositions.DownLeft;
                }
                return ThumbstickPositions.Left;
            }
            else
            {
                if (currentThumbstick.y > _sensitivy)
                {
                    return ThumbstickPositions.Up;
                }
                else if (currentThumbstick.y < -_sensitivy)
                {
                    return ThumbstickPositions.Down;
                }
            }
        }
        return ThumbstickPositions.None;
    }

}
