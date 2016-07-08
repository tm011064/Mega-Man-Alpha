using System;

[Serializable]
public class GameSettings
{
  public PowerUpSettings PowerUpSettings = new PowerUpSettings();

  public PlayerMetricsSettings PlayerMetricSettings = new PlayerMetricsSettings();
  
  public LogSettings LogSettings = new LogSettings();

  public PlayerDamageControlHandlerSettings PlayerDamageControlHandlerSettings = new PlayerDamageControlHandlerSettings();
}
