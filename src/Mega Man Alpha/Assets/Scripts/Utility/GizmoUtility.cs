using UnityEngine;

public static class GizmoUtility
{
  public static void DrawBoundingBox(Transform transform, Vector3 extents, Color color)
  {
    var frontTopLeft = new Vector3(-extents.x, +extents.y, -extents.z);  // front top left corner
    var frontTopRight = new Vector3(+extents.x, +extents.y, -extents.z);  // front top right corner
    var frontBottomLeft = new Vector3(-extents.x, -extents.y, -extents.z);  // front bottom left corner
    var frontBottomRight = new Vector3(+extents.x, -extents.y, -extents.z);  // front bottom right corner

    Gizmos.color = color;
    Gizmos.matrix = transform.localToWorldMatrix;

    Gizmos.DrawLine(frontTopLeft, frontTopRight);
    Gizmos.DrawLine(frontTopRight, frontBottomRight);
    Gizmos.DrawLine(frontBottomRight, frontBottomLeft);
    Gizmos.DrawLine(frontBottomLeft, frontTopLeft);
  }

  public static void DrawBoundingBox(Vector3 center, Vector3 extents, Color color)
  {
    var frontTopLeft = new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z);  // front top left corner
    var frontTopRight = new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z);  // front top right corner
    var frontBottomLeft = new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z);  // front bottom left corner
    var frontBottomRight = new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z);  // front bottom right corner

    Gizmos.color = color;

    Gizmos.DrawLine(frontTopLeft, frontTopRight);
    Gizmos.DrawLine(frontTopRight, frontBottomRight);
    Gizmos.DrawLine(frontBottomRight, frontBottomLeft);
    Gizmos.DrawLine(frontBottomLeft, frontTopLeft);
  }

  public static Rect BoundsToScreenRect(Bounds bounds)
  {
    // Get mesh origin and farthest extent (this works best with simple convex meshes)
    var origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
    var extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));

    // Create rect in screen space and return - does not account for camera perspective
    return new Rect(
      origin.x,
      Screen.height - origin.y,
      extent.x - origin.x,
      origin.y - extent.y);
  }

  public static Vector3[] CreateEllipse(float a, float b, float h, float k, float theta, int resolution)
  {
    var positions = new Vector3[resolution + 1];

    var quaternion = Quaternion.AngleAxis(theta, Vector3.forward);

    var center = new Vector3(h, k, 0.0f);
    for (var i = 0; i <= resolution; i++)
    {
      var angle = (float)i / (float)resolution * 2.0f * Mathf.PI;

      positions[i] = new Vector3(a * Mathf.Cos(angle), b * Mathf.Sin(angle), 0.0f);
      positions[i] = quaternion * positions[i] + center;
    }

    return positions;
  }
}
