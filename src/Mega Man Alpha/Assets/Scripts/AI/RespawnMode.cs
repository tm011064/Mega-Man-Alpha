public enum RespawnMode
{
  /// <summary>
  /// Spawn only one time
  /// </summary>
  SpawnOnce,
  /// <summary>
  /// Spawn continuously as long as the object is enabled
  /// </summary>
  SpawnContinuously,
  /// <summary>
  /// Spawn when the previously spawned object is destroyed
  /// </summary>
  SpawnWhenDestroyed
}
