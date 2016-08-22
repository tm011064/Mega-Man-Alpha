using UnityEngine;
public static class EdgeCollider2DExtensions
{
  public static Line2 ToLine2(this EdgeCollider2D self)
  {
    return new Line2(self.points[0], self.points[1]);
  }

  public static bool IsLeft(this EdgeCollider2D self, Vector2 point)
  {
    return self.ToLine2().IsLeft(point);
  }
}
