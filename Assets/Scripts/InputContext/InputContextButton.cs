using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputContextButton : MonoBehaviour {

    public enum ButtonState
    {
        NotPressed,
        Current,
        Pressed
    }

    private Image _buttonImage;
    private ButtonState _buttonState;
    private RectTransform _buttonRectTransform;

    public Sprite[] _buttonSprites;

    void Start()
    {
        _buttonImage = GetComponent<Image>();
        _buttonRectTransform = _buttonImage.rectTransform;
    }

    public void SetState(ButtonState state)
    {
        _buttonState = state;
        UpdateImage();
    }

    public void SetPosition(int index)
    {
        _buttonImage = GetComponent<Image>();
        _buttonRectTransform = _buttonImage.rectTransform;
        _buttonRectTransform.localPosition = new Vector3((42 + 2) * index, 0, 0);
    }

    public void SetRotation(InputContext.ThumbstickPositions position)
    {
        _buttonImage = GetComponent<Image>();
        _buttonRectTransform = _buttonImage.rectTransform;
        float angle = 0;
        switch (position)
        {
            case InputContext.ThumbstickPositions.Up:
                angle = 180;
            break;
            case InputContext.ThumbstickPositions.UpRight:
                angle = 135;
            break;
            case InputContext.ThumbstickPositions.Right:
                angle = 90;
            break;
            case InputContext.ThumbstickPositions.DownRight:
                angle = 45;
            break;
            case InputContext.ThumbstickPositions.Down:
                angle = 0;
            break;
            case InputContext.ThumbstickPositions.DownLeft:
                angle = -45;
            break;
            case InputContext.ThumbstickPositions.Left:
                angle = -90;
            break;
            case InputContext.ThumbstickPositions.UpLeft:
                angle = -135;
            break;
            case InputContext.ThumbstickPositions.None:
                angle = 0;
                break ;
        }
        _buttonImage.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateImage()
    {
        if(_buttonState == ButtonState.Current)
        {
            _buttonImage.sprite = _buttonSprites[0];
        }else if(_buttonState == ButtonState.NotPressed)
        {
            _buttonImage.sprite = _buttonSprites[1];
        }
        else
        {
            _buttonImage.sprite = _buttonSprites[2];
        }
    }
	
}
