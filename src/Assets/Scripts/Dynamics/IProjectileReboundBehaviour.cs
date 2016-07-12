using UnityEngine;

public interface IProjectileReboundBehaviour
{
  Vector3 HandleRebound(BoxCollider2D projectileCollider, Collider2D enemyCollider, Vector3 velocity);
}
