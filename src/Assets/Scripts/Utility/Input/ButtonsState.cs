using UnityEngine;

public class ButtonsState
{
  public ButtonPressState ButtonPressState;

  private string _buttonName;

  private float _pressStarted;

  public void Update()
  {
    ButtonPressState state = ButtonPressState.Idle;

    if (Input.GetButton(_buttonName))
    {
      state |= ButtonPressState.IsPressed;
    }

    if (((state & ButtonPressState.IsPressed) != 0)               // IF   currently pressed
      && ((ButtonPressState & ButtonPressState.IsPressed) == 0))  // AND  previously not pressed
    {
      _pressStarted = Time.time;

      state |= ButtonPressState.IsDown;
    }

    if (((state & ButtonPressState.IsPressed) == 0)               // IF   currently not pressed
      && ((ButtonPressState & ButtonPressState.IsPressed) != 0))  // AND  previously pressed
    {
      state |= ButtonPressState.IsUp;
    }

    if ((state & (ButtonPressState.IsUp | ButtonPressState.IsDown | ButtonPressState.IsPressed)) != 0)
    {
      state &= ~ButtonPressState.Idle;
    }

    ButtonPressState = state;
  }

  public float GetPressedTime()
  {
    if (((ButtonPressState & ButtonPressState.IsPressed) == 0))
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
