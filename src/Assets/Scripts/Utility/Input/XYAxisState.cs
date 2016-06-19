public struct XYAxisState
{
  public float XAxis;

  public float YAxis;

  public float SensitivityThreshold;

  /// <summary>
  /// If true, the horizontal axis press is not strong enough and must be ignored
  /// </summary>
  public bool IsInHorizontalSensitivityDeadZone()
  {
    return XAxis > -SensitivityThreshold
      && XAxis < SensitivityThreshold;
  }

  /// <summary>
  /// If true, the vertical axis press is not strong enough and must be ignored
  /// </summary>
  public bool IsInVerticalSensitivityDeadZone()
  {
    return YAxis > -SensitivityThreshold
      && YAxis < SensitivityThreshold;
  }
}
