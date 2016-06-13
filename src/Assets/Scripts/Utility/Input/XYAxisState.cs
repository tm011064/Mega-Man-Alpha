public struct XYAxisState
{
  public float XAxis;

  public float YAxis;

  public float SensibilityThreshold;

  public bool IsWithinThreshold()
  {
    return XAxis > -SensibilityThreshold
      && XAxis < SensibilityThreshold;
  }
}
