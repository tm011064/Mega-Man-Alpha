using System;
using UnityEngine;

[Serializable]
public class SlideSettings
{
  public bool EnableSliding = true;

  public float Duration = 1f;

  public float Distance = 200f;

  [Tooltip("You can pass in any button name as well as the following axis directions: Left, Right, Up, Down")]
  public string[] InputButtonsPressed = new string[] { "Down" };

  public string InputButtonDown = "Jump";
}