using System;
using UnityEngine;

[Serializable]
public class GridLineSettings
{
  [Range(16, 2048)]
  public float Width = 32f;

  [Range(16, 2048)]
  public float Height = 32f;

  public bool Visible = false;
}