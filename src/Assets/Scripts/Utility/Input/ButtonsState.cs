using UnityEngine;

public class ButtonsState
{
  private const string TRACE_TAG = "ButtonsState";

  private string _buttonName;

  private float _pressStarted;

  public ButtonPressState buttonPressState;

  public void Update()
  {
    ButtonPressState state = ButtonPressState.Idle;

    if (Input.GetButton(_buttonName))
    {
      state |= ButtonPressState.IsPressed;
    }

    if (((state & ButtonPressState.IsPressed) != 0)               // IF   currently pressed
      && ((buttonPressState & ButtonPressState.IsPressed) == 0))  // AND  previously not pressed
    {
      _pressStarted = Time.time;

      state |= ButtonPressState.IsDown;

      Logger.Trace(TRACE_TAG, "Button " + _buttonName + " is down.");
    }

    if (((state & ButtonPressState.IsPressed) == 0)               // IF   currently not pressed
      && ((buttonPressState & ButtonPressState.IsPressed) != 0))  // AND  previously pressed
    {
      state |= ButtonPressState.IsUp;
    }

    if ((state & (ButtonPressState.IsUp | ButtonPressState.IsDown | ButtonPressState.IsPressed)) != 0)
    {
      state &= ~ButtonPressState.Idle;
    }

    buttonPressState = state;
  }

  public float GetPressedTime()
  {
    if (((buttonPressState & ButtonPressState.IsPressed) == 0))
    {
      return 0f;
    }

    return Time.time - _pressStarted;
  }

  public ButtonsState(string buttonName)
  {
    _buttonName = buttonName;
  }
}
