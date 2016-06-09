#if UNITY_EDITOR

using UnityEngine;

public partial class DeactivatePooledObjectTrigger : MonoBehaviour
{
  public Color OutlineGizmoColor = Color.magenta;

  public bool ShowGizmoOutline = true;

  private Vector3 _gizmoCenter = Vector3.zero;

  private Vector3 _gizmoExtents = new Vector3(16, 16, 0);

  private BoxCollider2D _boxCollider2D;

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      _boxCollider2D = GetComponent<BoxCollider2D>();

      if (_boxCollider2D != null)
      {
        _gizmoCenter = _boxCollider2D.offset;
        _gizmoExtents = _boxCollider2D.size / 2;
      }

      GizmoUtility.DrawBoundingBox(transform.TransformPoint(_gizmoCenter), _gizmoExtents, OutlineGizmoColor);
    }
  }
}

#endif