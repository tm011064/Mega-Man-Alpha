using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public partial class CameraTrolley : MonoBehaviour
{
  public int NodeCount;

  [HideInInspector]
  public List<Vector3> Nodes = new List<Vector3>() { Vector3.zero, Vector3.zero };

  [HideInInspector]
  public bool IsPlayerWithinBoundingBox;

  private Vector3[] _worldCoordinateNodes = null;

  void OnTriggerEnter2D(Collider2D col)
  {
    IsPlayerWithinBoundingBox = true;
  }

  void OnTriggerExit2D(Collider2D col)
  {
    IsPlayerWithinBoundingBox = false;
  }

  void OnEnable()
  {
    if (_worldCoordinateNodes == null)
    {
      _worldCoordinateNodes = new Vector3[Nodes.Count];

      for (var i = 0; i < Nodes.Count; i++)
      {
        _worldCoordinateNodes[i] = transform.TransformPoint(Nodes[i]);

        if (i > 0 && _worldCoordinateNodes[i - 1].x == _worldCoordinateNodes[i].x)
        {
          throw new ArgumentOutOfRangeException("Vertical lines not supported.");
        }
      }
    }
  }

  public float? GetPositionY(float posX)
  {
    for (var i = 1; i < NodeCount; i++)
    {
      if (_worldCoordinateNodes[i - 1].x <= posX
        && _worldCoordinateNodes[i].x >= posX)
      {
        var deltaMovement =
          (_worldCoordinateNodes[i - 1].y - _worldCoordinateNodes[i].y)
          / (_worldCoordinateNodes[i - 1].x - _worldCoordinateNodes[i].x);

        return deltaMovement * posX
          - deltaMovement * _worldCoordinateNodes[i - 1].x
          + _worldCoordinateNodes[i - 1].y;
      }
    }

    return null;
  }
}
