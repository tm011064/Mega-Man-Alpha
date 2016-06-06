using System;
using UnityEngine;

[Serializable]
public class VisibleRectangle
{
  public bool IsEnabled;

  public bool IsInsideOut;

  public Vector2 LeftTop = Vector2.zero;

  public int Width;

  public int Height;
}