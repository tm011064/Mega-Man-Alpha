using System;

[Serializable]
public class GameSettings
{
  public PlayerMetricsSettings PlayerMetricSettings = new PlayerMetricsSettings();

  public LogSettings LogSettings = new LogSettings();

  public ObjectPoolSettings ObjectPoolSettings = new ObjectPoolSettings();
}
