#if UNITY_EDITOR

using UnityEngine;

public partial class AttachPlayerControllerToTrampoline : MonoBehaviour
{
  public Color OutlineGizmoColor = Color.yellow;

  public bool ShowGizmoOutline = true;

  private Vector3 _gizmoCenter = Vector3.zero;

  private Vector3 _gizmoExtents = new Vector3(16, 16, 0);

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      if (trampolinePrefab != null)
      {
        var boxCollider = trampolinePrefab.GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
          _gizmoCenter = boxCollider.offset;
          _gizmoExtents = boxCollider.size / 2;
        }
      }

      GizmoUtility.DrawBoundingBox(transform.TransformPoint(_gizmoCenter), _gizmoExtents, OutlineGizmoColor);
    }
  }
}

#endif