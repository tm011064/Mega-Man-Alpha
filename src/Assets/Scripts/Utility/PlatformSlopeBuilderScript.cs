using System.Collections.Generic;
using UnityEngine;

public class PlatformSlopeBuilderScript : MonoBehaviour
{
  public GameObject Platform;

  public Vector2 PlatformDimensions;

  public Vector3 SpawnPoint;

  public float[] Angles;

  private Vector2 GetMidPoint(float x, float y, float width, float height, float angle_degrees)
  {
    var angle_rad = angle_degrees * Mathf.PI / 180f;

    var cosa = Mathf.Cos(angle_rad);

    var sina = Mathf.Sin(angle_rad);

    var widthHalf = width / 2f;

    var heightHalf = height / 2f;

    return new Vector2(
      x + widthHalf * cosa + heightHalf * sina,
      y + widthHalf * sina - heightHalf * cosa);
  }

  public void BuildObject()
  {
    Logger.Info("Building slope platform");

    var rotationsQueue = new Queue<float>(Angles);

    var currentRightTop = new Vector2(SpawnPoint.x, SpawnPoint.y);

    while (rotationsQueue.Count > 0)
    {
      var angle = rotationsQueue.Dequeue();

      var midPoint = GetMidPoint(currentRightTop.x, currentRightTop.y, PlatformDimensions.x, PlatformDimensions.y, angle);

      var normalRightTop = new Vector2(currentRightTop.x + PlatformDimensions.x, currentRightTop.y);

      var angle_rad = angle * Mathf.PI / 180f;

      var nextRightTop = new Vector2(
        Mathf.Cos(angle_rad) * (normalRightTop.x - currentRightTop.x) - Mathf.Sin(angle_rad) * (normalRightTop.y - currentRightTop.y) + currentRightTop.x,
        Mathf.Sin(angle_rad) * (normalRightTop.x - currentRightTop.x) - Mathf.Cos(angle_rad) * (normalRightTop.y - currentRightTop.y) + currentRightTop.y);

      currentRightTop = nextRightTop;

      var gameObject = Instantiate(
        Platform,
        new Vector3(midPoint.x, midPoint.y, 0f), Quaternion.Euler(0, 0, angle)) as GameObject;

      var groundPlatformSpriteRenderer = gameObject.GetComponent<GroundPlatformSpriteRenderer>();

      if (groundPlatformSpriteRenderer != null)
      {
        groundPlatformSpriteRenderer.Width = Mathf.RoundToInt(PlatformDimensions.x);
        groundPlatformSpriteRenderer.Height = Mathf.RoundToInt(PlatformDimensions.y);
      }
    }
  }
}
