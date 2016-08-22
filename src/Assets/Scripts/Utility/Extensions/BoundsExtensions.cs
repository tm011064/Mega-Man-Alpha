using UnityEngine;

public static class BoundsExtensions
{
  public static Rect ToRect(this Bounds self)
  {
    return new Rect(self.center, self.size);
  }

  public static bool AreWithinVerticalShaftOf(this Bounds self, Bounds bounds)
  {
    return self.center.x - self.extents.x >= bounds.center.x - bounds.extents.x
      && self.center.x + self.extents.x <= bounds.center.x + bounds.extents.x;
  }

  public static bool AreAbove(this Bounds self, Bounds bounds)
  {
    return self.center.y - self.extents.y > bounds.center.y + bounds.extents.y;
  }

  public static bool AreAboveOrOnEdge(this Bounds self, Bounds bounds)
  {
    float selfBottom = self.center.y - self.extents.y;
    float boundsTop = bounds.center.y + bounds.extents.y;

    return selfBottom > boundsTop
      || Mathf.Approximately(selfBottom, boundsTop);
  }

  public static bool AreBelow(this Bounds self, Bounds bounds)
  {
    return self.center.y + self.extents.y < bounds.center.y - bounds.extents.y;
  }

  public static bool ContainBottomEdgeOf(this Bounds self, Bounds bounds)
  {
    return self.center.y + self.extents.y >= bounds.center.y - bounds.extents.y
      && self.center.x - self.extents.x <= bounds.center.x - bounds.extents.x
      && self.center.x + self.extents.x >= bounds.center.x + bounds.extents.x;
  }

  public static bool ContainTopEdgeOf(this Bounds self, Bounds bounds)
  {
    return self.center.y - self.extents.y <= bounds.center.y + bounds.extents.y
      && self.center.x - self.extents.x <= bounds.center.x - bounds.extents.x
      && self.center.x + self.extents.x >= bounds.center.x + bounds.extents.x;
  }

  public static bool ContainRightEdgeOf(this Bounds self, Bounds bounds)
  {
    return self.center.x + self.extents.x >= bounds.center.x + bounds.extents.x
      && self.center.y + self.extents.y >= bounds.center.y - bounds.extents.y
      && self.center.y - self.extents.y <= bounds.center.y + bounds.extents.y;
  }

  public static bool ContainLeftEdgeOf(this Bounds self, Bounds bounds)
  {
    return self.center.y - self.extents.y <= bounds.center.y + bounds.extents.y
      && self.center.y + self.extents.y >= bounds.center.y - bounds.extents.y
      && self.center.y - self.extents.y <= bounds.center.y + bounds.extents.y;
  }
}
