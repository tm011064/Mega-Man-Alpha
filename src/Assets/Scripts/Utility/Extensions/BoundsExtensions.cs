using UnityEngine;

public static class BoundsExtensions
{
  public static bool AreWithinVerticalShaftOf(this Bounds self, Bounds bounds)
  {
    return self.center.x - self.extents.x >= bounds.center.x - bounds.extents.x
      && self.center.x + self.extents.x <= bounds.center.x + bounds.extents.x;
  }

  public static bool AreAbove(this Bounds self, Bounds bounds)
  {
    return self.center.y - self.extents.y > bounds.center.y + bounds.extents.y;
  }

  public static bool AreBelow(this Bounds self, Bounds bounds)
  {
    return self.center.y + self.extents.y < bounds.center.y - bounds.extents.y;
  }
}
