using UnityEngine;

public class AxisState
{
  public float Value;

  public float LastValue;

  private string _axisName;

  public void Update()
  {
    LastValue = Value;

    Value = Input.GetAxis(_axisName);
  }

  public AxisState Clone()
  {
    return (AxisState)MemberwiseClone();
  }

  public AxisState(float value)
  {
    this.Value = value;
  }

  public AxisState(string axisName)
  {
    _axisName = axisName;
  }
}
