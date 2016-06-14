using UnityEngine;

public static class BoundsExtensions
{
  public static bool IsSurroundedBy(this Bounds self, Bounds bounds)
  {
    return self.center.x - self.extents.x >= bounds.center.x - bounds.extents.x
      && self.center.x + self.extents.x <= bounds.center.x + bounds.extents.x
      && self.center.y + self.extents.y <= bounds.center.y + bounds.extents.y
      && self.center.y - self.extents.y >= bounds.center.y - bounds.extents.y;
  }

  public static bool AreWithinVerticalBoundsOf(this Bounds self, Bounds bounds)
  {
    return self.center.x - self.extents.x >= bounds.center.x - bounds.extents.x
      && self.center.x + self.extents.x <= bounds.center.x + bounds.extents.x;
  }

  public static bool AreOutsideTopHorizontalBoundsOf(this Bounds self, Bounds bounds)
  {
    return self.center.y - self.extents.y > bounds.center.y + bounds.extents.y;
  }

  public static bool AreOutsideBottomHorizontalBoundsOf(this Bounds self, Bounds bounds)
  {
    return self.center.y + self.extents.y < bounds.center.y - bounds.extents.y;
  }
}
