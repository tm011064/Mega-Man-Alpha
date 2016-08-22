using UnityEngine;

public static class Line2Extensions
{
  public static bool IsLeft(this Line2 self, Vector2 point)
  {
    return (
              (self.To.x - self.From.x) * (point.y - self.From.y)
            - (self.To.y - self.From.y) * (point.x - self.From.x)
           ) > 0;
  }

  public static bool Intersects(this Line2 self, Line2 line)
  {
    Vector2 a = self.To - self.From;
    Vector2 b = line.From - line.To;
    Vector2 c = self.From - line.From;

    float alphaNumerator = b.y * c.x - b.x * c.y;
    float alphaDenominator = a.y * b.x - a.x * b.y;
    float betaNumerator = a.x * c.y - a.y * c.x;
    float betaDenominator = a.y * b.x - a.x * b.y;

    if (alphaDenominator == 0 || betaDenominator == 0)
    {
      return false;
    }

    if (alphaDenominator > 0)
    {
      if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
      {
        return false;
      }
    }
    else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
    {
      return false;
    }

    if (betaDenominator > 0)
    {
      if (betaNumerator < 0 || betaNumerator > betaDenominator)
      {
        return false;
      }
    }
    else if (betaNumerator > 0 || betaNumerator < betaDenominator)
    {
      return false;
    }

    return true;
  }
}
