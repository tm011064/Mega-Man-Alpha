using System.Collections.Generic;

public class InputStateManager
{
  private Dictionary<string, ButtonsState> _buttonStates;

  private Dictionary<string, AxisState> _axisStates;

  public InputStateManager()
  {
    _buttonStates = new Dictionary<string, ButtonsState>();
    _axisStates = new Dictionary<string, AxisState>();
  }

  public ButtonsState GetButtonState(string buttonName)
  {
    return _buttonStates[buttonName];
  }

  public AxisState GetVerticalAxisState()
  {
    return _axisStates["Vertical"];
  }

  public AxisState GetHorizontalAxisState()
  {
    return _axisStates["Horizontal"];
  }

  public void Update()
  {
    foreach (var value in _buttonStates.Values)
    {
      value.Update();
    }

    foreach (var value in _axisStates.Values)
    {
      value.Update();
    }
  }

  public void InitializeButtons(params string[] buttonNames)
  {
    for (var i = 0; i < buttonNames.Length; i++)
    {
      _buttonStates[buttonNames[i]] = new ButtonsState(buttonNames[i]);
    }
  }

  public void InitializeAxes(params string[] azisNames)
  {
    for (var i = 0; i < azisNames.Length; i++)
    {
      _axisStates[azisNames[i]] = new AxisState(azisNames[i]);
    }
  }
}
