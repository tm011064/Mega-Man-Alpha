using System;
using UnityEngine;

public class FPSRenderer
{
  private static Texture2D _staticRectTexture;

  private static GUIStyle _staticRectStyle;

  public float SceneStartTime;

  private float _framesPerSecond;

  private float _time;

  private int _frames;

  void InitFPS()
  {
    _framesPerSecond = 0.0f;

    _time = 0.0f;

    _frames = 0;
  }

  public void UpdateFPS()
  {
    _frames++;

    _time += Time.deltaTime;

    if (_time > 1.0f)
    {
      _framesPerSecond = _frames;

      _time -= 1.0f;

      _frames = 0;
    }
  }

  public void RenderFPS()
  {
    GUIDrawRect(new Rect(0, 0, 64, 40), Color.red);

    GUI.Label(new Rect(4, 0, 60, 22), "FPS: " + (int)_framesPerSecond);

    TimeSpan sceneRunTime = TimeSpan.FromSeconds(Time.time - SceneStartTime);

    GUI.Label(
      new Rect(4, 18, 120, 22),
      sceneRunTime.Minutes.ToString("00") + ":" + sceneRunTime.Seconds.ToString("00") + "." + sceneRunTime.Milliseconds.ToString("000"));
  }

  public static void GUIDrawRect(Rect position, Color color)
  {
    if (_staticRectTexture == null)
    {
      _staticRectTexture = new Texture2D(1, 1);
    }

    if (_staticRectStyle == null)
    {
      _staticRectStyle = new GUIStyle();
    }

    _staticRectTexture.SetPixel(0, 0, color);

    _staticRectTexture.Apply();

    _staticRectStyle.normal.background = _staticRectTexture;

    GUI.Box(position, GUIContent.none, _staticRectStyle);
  }
}
