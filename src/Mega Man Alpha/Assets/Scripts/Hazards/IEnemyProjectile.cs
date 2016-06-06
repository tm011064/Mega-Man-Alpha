using UnityEngine;

public interface IEnemyProjectile
{
  void StartMove(Vector2 startPosition, Vector2 direction, float acceleration, float targetVelocity);
}