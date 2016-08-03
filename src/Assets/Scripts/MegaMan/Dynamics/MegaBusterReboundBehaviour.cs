using UnityEngine;

public class MegaBusterReboundBehaviour : MonoBehaviour, IProjectileReboundBehaviour
{
  private void MoveProjectileOutsideOfEnemyCollider(BoxCollider2D projectileCollider, Collider2D enemyCollider, Vector3 velocity)
  {
    var deltaX = 0f;

    if (velocity.x > 0f)
    {
      deltaX = enemyCollider.bounds.min.x - projectileCollider.bounds.extents.x - projectileCollider.bounds.center.x;
    }
    else if (velocity.x < 0f)
    {
      deltaX = enemyCollider.bounds.max.x + projectileCollider.bounds.extents.x - projectileCollider.bounds.center.x;
    }

    var vector = new Vector2(deltaX, 0f) * 1.001f; // smudge factor

    transform.Translate(vector, Space.World);
  }

  public Vector3 HandleRebound(BoxCollider2D projectileCollider, Collider2D enemyCollider, Vector3 velocity)
  {
    MoveProjectileOutsideOfEnemyCollider(projectileCollider, enemyCollider, velocity);

    var zRotationAngle = (velocity.x > 0f) ? 135f : -135f;

    return Quaternion.Euler(0, 0, zRotationAngle) * velocity;
  }
}
