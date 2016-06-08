using System;
using UnityEngine;
public class StationaryLaser : SpawnBucketItemBehaviour
{
  private LineRenderer _lineRenderer;

  [Tooltip("All layers that the scan rays can collide with. Should include platforms and player.")]
  public LayerMask ScanRayCollisionLayers = 0;

  [Tooltip("The direction of the laser emitted from the game object's position.")]
  public Direction Direction;

  [Tooltip("The offset of the point where the laser gets emitted.")]
  public Vector3 ScanRayEmissionPositionOffset = Vector3.zero;

  void Awake()
  {
    _lineRenderer = GetComponent<LineRenderer>();
  }

  void Update()
  {
    Vector2 vector;

    float magnitude;

    var startPosition = transform.position;

    switch (Direction)
    {
      case Direction.Bottom:

        vector = -Vector2.up;

        magnitude = Screen.height;

        startPosition += ScanRayEmissionPositionOffset;

        break;

      case Direction.Left:

        vector = -Vector2.right;

        magnitude = Screen.width;

        startPosition += ScanRayEmissionPositionOffset;

        break;

      case Direction.Right:

        vector = Vector2.right;

        magnitude = Screen.width;

        startPosition += ScanRayEmissionPositionOffset;

        break;

      case Direction.Top:

        vector = Vector2.up;

        magnitude = Screen.height;

        startPosition += ScanRayEmissionPositionOffset;

        break;

      default:
        throw new ArgumentException("Direction " + Direction + " not supported.");
    }

    var raycastHit = Physics2D.Raycast(startPosition, vector.normalized, magnitude, ScanRayCollisionLayers);

    var hasHit = raycastHit == true;

    if (hasHit
      && raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
    {
      GameManager.Instance.PowerUpManager.KillPlayer();

      hasHit = false;
    }

    if (_lineRenderer.useWorldSpace)
    {
      if (hasHit)
      {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, raycastHit.point);
      }
      else
      {
        _lineRenderer.SetPosition(0, transform.position);

        switch (Direction)
        {
          case Direction.Bottom:
            _lineRenderer.SetPosition(1, transform.position - new Vector3(0f, -Screen.height));
            break;

          case Direction.Left:
            _lineRenderer.SetPosition(1, transform.position - new Vector3(-Screen.width, 0));
            break;

          case Direction.Right:
            _lineRenderer.SetPosition(1, transform.position - new Vector3(Screen.width, 0f));
            break;

          case Direction.Top:
            _lineRenderer.SetPosition(1, transform.position - new Vector3(0f, Screen.height));
            break;

          default:
            throw new ArgumentException("Direction " + Direction + " not supported.");
        }
      }
    }
    else
    {
      if (hasHit)
      {
        _lineRenderer.SetPosition(0, Vector3.zero);
        _lineRenderer.SetPosition(1, raycastHit.point.ToVector3() - transform.position);
      }
      else
      {
        _lineRenderer.SetPosition(0, Vector3.zero);

        switch (Direction)
        {
          case Direction.Bottom:
            _lineRenderer.SetPosition(1, new Vector3(0f, -Screen.height));
            break;

          case Direction.Left:
            _lineRenderer.SetPosition(1, new Vector3(-Screen.width, 0));
            break;

          case Direction.Right:
            _lineRenderer.SetPosition(1, new Vector3(Screen.width, 0f));
            break;

          case Direction.Top:
            _lineRenderer.SetPosition(1, new Vector3(0f, Screen.height));
            break;

          default:
            throw new ArgumentException("Direction " + Direction + " not supported.");
        }
      }
    }
  }
}
