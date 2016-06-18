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

  static bool DoLinesIntersect(Vector2 firstLineFrom, Vector2 firstLineTo, Vector2 p3, Vector2 p4)
  {
    Vector2 a = firstLineTo - firstLineFrom;
    Vector2 b = p3 - p4;
    Vector2 c = firstLineFrom - p3;

    float alphaNumerator = b.y * c.x - b.x * c.y;
    float alphaDenominator = a.y * b.x - a.x * b.y;
    float betaNumerator = a.x * c.y - a.y * c.x;
    float betaDenominator = a.y * b.x - a.x * b.y;

    bool doIntersect = true;

    if (alphaDenominator == 0 || betaDenominator == 0)
    {
      doIntersect = false;
    }
    else
    {
      if (alphaDenominator > 0)
      {
        if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
        {
          doIntersect = false;

        }
      }
      else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
      {
        doIntersect = false;
      }

      if (doIntersect && betaDenominator > 0)
      {
        if (betaNumerator < 0 || betaNumerator > betaDenominator)
        {
          doIntersect = false;
        }
      }
      else if (betaNumerator > 0 || betaNumerator < betaDenominator)
      {
        doIntersect = false;
      }
    }

    return doIntersect;
  }

}
