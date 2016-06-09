using System;
using UnityEngine;

public static class CollisionDetectionUtility
{
  private const float K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR = 0.001f;

  public static float GetVerticalDistanceBetweenRays(BoxCollider2D boxCollider2D, Vector3 transformScale, int totalRays, float skinWidth)
  {
    return (boxCollider2D.size.y * Mathf.Abs(transformScale.y) - (2f * skinWidth)) / (totalRays - 1);
  }

  public static float GetHorizontalDistanceBetweenRays(BoxCollider2D boxCollider2D, Vector3 transformScale, int totalRays, float skinWidth)
  {
    return (boxCollider2D.size.x * Mathf.Abs(transformScale.x) - (2f * skinWidth)) / (totalRays - 1);
  }

  public static Vector3 AdjustDeltaMovementWithCollisionCheck(
    AxisType axisType,
    BoxCollider2D boxCollider,
    LayerMask raycastLayerMask,
    Vector3 deltaMovement,
    float skinWidth,
    int totalRays,
    float distanceBetweenRays)
  {
    if ((axisType == AxisType.Horizontal && deltaMovement.x == 0f)
      || (axisType == AxisType.Vertical && deltaMovement.y == 0f))
    {// if we don't move along the axis we don't check for collisions
      return deltaMovement;
    }

    var modifiedBounds = boxCollider.bounds;
    modifiedBounds.Expand(-2f * skinWidth);

    RaycastOrigins raycastOrigins;
    raycastOrigins.TopLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);
    raycastOrigins.BottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);
    raycastOrigins.BottomLeft = modifiedBounds.min;

    if (axisType == AxisType.Horizontal)
    {
      var originX = deltaMovement.x < 0f ? raycastOrigins.TopLeft.x : raycastOrigins.BottomRight.x;

      var initialRayOrigin = deltaMovement.x > 0f ? raycastOrigins.BottomRight : raycastOrigins.BottomLeft;

      float? minTranslateX = null;

      for (var i = 0; i < totalRays; i++)
      {
        var origin = new Vector2(originX, initialRayOrigin.y + i * distanceBetweenRays);

        var raycastHit = Physics2D.Raycast(
          origin,
          deltaMovement.x >= 0f ? Vector2.right : -Vector2.right,
          Mathf.Abs(deltaMovement.x) + skinWidth,
          raycastLayerMask);

        DrawRay(origin, new Vector2(deltaMovement.x + (deltaMovement.x > 0f ? -1f : 1f) * skinWidth, 0f), Color.magenta);

        if (raycastHit)
        {
          var translateX = raycastHit.point.x - origin.x + (skinWidth * deltaMovement.x > 0 ? -1f : 1f);

          if (!minTranslateX.HasValue
            || (translateX < 0f && translateX > minTranslateX)
            || (translateX > 0f && translateX < minTranslateX))
          {
            minTranslateX = translateX;

            if (Mathf.Abs(minTranslateX.Value) < K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
            {
              break;
            }
          }
        }
      }

      if (!minTranslateX.HasValue)
      {
        return deltaMovement;
      }
      else
      {
        return new Vector3(minTranslateX.Value, 0f, 0f);
      }
    }
    else if (axisType == AxisType.Vertical)
    {
      var originY = deltaMovement.y < 0f ? raycastOrigins.BottomRight.y : raycastOrigins.TopLeft.y;

      var initialRayOrigin = deltaMovement.y > 0f ? raycastOrigins.TopLeft : raycastOrigins.BottomLeft;

      float? minTranslateY = null;

      for (var i = 0; i < totalRays; i++)
      {
        var origin = new Vector2(initialRayOrigin.x + i * distanceBetweenRays, originY);

        var raycastHit = Physics2D.Raycast(
          origin,
          deltaMovement.y >= 0f ? Vector2.up : -Vector2.up,
          Mathf.Abs(deltaMovement.y) + skinWidth,
          raycastLayerMask);

        DrawRay(origin, new Vector2(0f, deltaMovement.x + (deltaMovement.y > 0f ? -1f : 1f) * skinWidth), Color.magenta);

        if (raycastHit)
        {
          var translateY = raycastHit.point.y - origin.y + (skinWidth * deltaMovement.y > 0 ? -1f : 1f);

          if (!minTranslateY.HasValue
            || (translateY < 0f && translateY > minTranslateY)
            || (translateY > 0f && translateY < minTranslateY))
          {
            minTranslateY = translateY;

            if (Mathf.Abs(minTranslateY.Value) < K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
            {
              break;
            }
          }
        }
      }

      if (!minTranslateY.HasValue)
      {
        return deltaMovement;
      }
      else
      {
        return new Vector3(0f, minTranslateY.Value, 0f);
      }
    }
    else
    {
      throw new NotImplementedException("Axis type " + axisType + " is not supported.");
    }
  }

  [System.Diagnostics.Conditional("DEBUG")]
  private static void DrawRay(Vector3 start, Vector3 dir, Color color)
  {
    Debug.DrawRay(start, dir, color);
  }

  private struct RaycastOrigins
  {
    public Vector2 TopLeft;

    public Vector2 BottomRight;

    public Vector2 BottomLeft;
  }
}
