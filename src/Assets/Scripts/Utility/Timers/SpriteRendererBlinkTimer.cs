using UnityEngine;

public class SpriteRendererBlinkTimer : IntervalTimer
{
  private readonly SpriteRenderer _spriteRenderer;

  public SpriteRendererBlinkTimer(float interval, SpriteRenderer spriteRenderer)
    : base(interval)
  {
    _spriteRenderer = spriteRenderer;
  }

  public void ShowSprite()
  {
    _spriteRenderer.color = new Color(
     _spriteRenderer.color.r,
     _spriteRenderer.color.g,
     _spriteRenderer.color.b,
     1f);
  }

  public override void Callback()
  {
    _spriteRenderer.color = new Color(
      _spriteRenderer.color.r,
      _spriteRenderer.color.g,
      _spriteRenderer.color.b,
      _spriteRenderer.color.a > 0f ? 0f : 1f);
  }
}