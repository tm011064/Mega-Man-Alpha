using UnityEngine;

public struct Line2
{
  public Vector2 From;

  public Vector2 To;

  public Line2(Vector2 from, Vector2 to)
  {
    From = from;
    To = to;
  }

  public Line2(Vector2[] points)
  {
    From = points[0];
    To = points[1];
  }

  public Vector2[] ToVectors()
  {
    return new Vector2[] { From, To };
  }

  public override string ToString()
  {
    return "line from " + From.ToString() + " to " + To.ToString();
  }
}
