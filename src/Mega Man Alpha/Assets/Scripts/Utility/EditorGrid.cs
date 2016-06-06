using System.Collections.Generic;
using UnityEngine;

public class EditorGrid : MonoBehaviour
{
  public List<GridLineSettings> GridLineSettings = new List<GridLineSettings>();

  public bool Visible = true;

  public Vector2 Size = new Vector2(2048, 2048);

  public Vector2 Center = Vector2.zero;

  void OnDrawGizmos()
  {
    if (Visible)
    {
      var white = Color.white;

      white.a = .4f;

      var gray = Color.white;

      gray.a = .2f;

      var colorQueue = new Queue<Color>(new List<Color>() { gray, white, Color.green });

      for (var i = 0; i < GridLineSettings.Count; i++)
      {
        if (!GridLineSettings[i].Visible)
        {
          continue;
        }

        if ((int)GridLineSettings[i].Height < 16 || (int)GridLineSettings[i].Width < 16)
        {
          continue;
        }

        if (colorQueue.Count > 0)
        {
          Gizmos.color = colorQueue.Dequeue();
        }

        var leftX = Mathf.RoundToInt(Center.x - Size.x * .5f);

        var rightX = Mathf.RoundToInt(Center.x + Size.x * .5f);

        var bottomY = Mathf.RoundToInt(Center.y - Size.y * .5f);

        var topY = Mathf.RoundToInt(Center.y + Size.y * .5f);

        for (int y = bottomY; y < topY; y += (int)GridLineSettings[i].Height)
        {
          Gizmos.DrawLine(
            new Vector3(leftX, y),
            new Vector3(rightX, y));
        }

        for (int x = leftX; x < rightX; x += (int)GridLineSettings[i].Width)
        {
          Gizmos.DrawLine(
            new Vector3(x, bottomY),
            new Vector3(x, topY));
        }
      }
    }
  }
}
