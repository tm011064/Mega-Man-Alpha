using UnityEngine;

public static class RectangleExtensions
{
  public static bool Intersects(this Rect self, Rect other)
  {
    if (self.max.x < other.min.x
      || self.max.y < other.min.y
      || self.min.x > other.max.x
      || self.min.y > other.max.y)
    {
      return false;
    }

    return true;
  }
}
