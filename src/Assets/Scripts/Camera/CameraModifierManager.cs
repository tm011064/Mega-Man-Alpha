using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraModifierManager
{
  private readonly List<CameraModifierListItem> _cameraModifiers = new List<CameraModifierListItem>();

  public void EnterCameraModifier(
    MonoBehaviour monoBehaviour,
    Collider2D collider2D,
    Vector2 playerPosition,
    CameraMovementSettings cameraMovementSettings)
  {
    var cameraModifierListItem = new CameraModifierListItem(
      monoBehaviour,
      collider2D,
      playerPosition,
      cameraMovementSettings);

    _cameraModifiers.Add(cameraModifierListItem);
  }

  public CameraMovementSettings ExitCameraModifier(
    MonoBehaviour monoBehaviour,
    Collider2D collider2D,
    Vector2 playerPosition)
  {
    var lastCameraModifierListItem = _cameraModifiers.Last();

    if (lastCameraModifierListItem.Equals(monoBehaviour, collider2D))
    {
      if (lastCameraModifierListItem.ColliderIntersects(playerPosition))
      {
        _cameraModifiers.Clear();

        _cameraModifiers.Add(lastCameraModifierListItem);

        return lastCameraModifierListItem.CameraMovementSettings;
      }

      // player exited the latest edge collider at the same side, so we just need to remove
      // the item and return the camera modifier settings we had prio to the player entering
      // the edge collider
      _cameraModifiers.Remove(lastCameraModifierListItem);
    }

    return _cameraModifiers.Last().CameraMovementSettings;
  }

  private class CameraModifierListItem
  {
    private readonly MonoBehaviour _monoBehaviour;

    private readonly Collider2D _collider2D;

    private readonly Vector2 _startPlayerPosition;

    public CameraModifierListItem(
      MonoBehaviour monoBehaviour,
      Collider2D collider2D,
      Vector2 playerPosition,
      CameraMovementSettings cameraMovementSettings)
    {
      _monoBehaviour = monoBehaviour;
      _collider2D = collider2D;
      _startPlayerPosition = playerPosition;

      CameraMovementSettings = cameraMovementSettings;
    }

    public CameraMovementSettings CameraMovementSettings { get; private set; }

    public bool ColliderIntersects(Vector2 currentPlayerPosition)
    {
      var edgeCollider2D = _collider2D as EdgeCollider2D;

      if (edgeCollider2D != null)
      {
        var edgeColliderLine = new Line2(edgeCollider2D.points);
        var playerLine = new Line2(_startPlayerPosition, currentPlayerPosition);

        return playerLine.Intersects(edgeColliderLine);
      }

      var boxCollider2D = _collider2D as BoxCollider2D;

      if (boxCollider2D != null)
      {
        var playerMovementRectangle = new Rect(
          _startPlayerPosition.x + (currentPlayerPosition.x - _startPlayerPosition.x) / 2,
          _startPlayerPosition.y + (currentPlayerPosition.y - _startPlayerPosition.y) / 2,
          (currentPlayerPosition.x - _startPlayerPosition.x),
          (currentPlayerPosition.y - _startPlayerPosition.y));

        return playerMovementRectangle.Intersects(boxCollider2D.bounds.ToRect());
      }

      throw new NotSupportedException();
    }

    public bool Equals(MonoBehaviour monoBehaviour, Collider2D collider2D)
    {
      return _monoBehaviour == monoBehaviour
        && _collider2D == collider2D;
    }
  }
}