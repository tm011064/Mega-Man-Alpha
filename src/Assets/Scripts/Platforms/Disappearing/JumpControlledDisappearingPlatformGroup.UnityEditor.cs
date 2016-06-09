#if UNITY_EDITOR

using System;
using UnityEngine;

public partial class JumpControlledDisappearingPlatformGroup : MonoBehaviour
{
  public Color OutlineGizmoColor = Color.yellow;

  public bool ShowGizmoOutline = true;

  private Vector3 _gizmoCenter = Vector3.zero;

  private Vector3 _gizmoExtents = new Vector3(16, 16, 0);

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      if (PlatformPrefab != null)
      {
        var rectangleMeshBuildScript = PlatformPrefab.GetComponent<RectangleMeshBuildScript>();

        if (rectangleMeshBuildScript != null)
        {
          switch (rectangleMeshBuildScript.Anchor)
          {
            case TextAnchor.MiddleCenter:

              _gizmoCenter = Vector3.zero;

              _gizmoExtents = new Vector3(rectangleMeshBuildScript.Width / 2, rectangleMeshBuildScript.Height / 2);

              break;

            case TextAnchor.LowerLeft:

              _gizmoCenter = new Vector3(rectangleMeshBuildScript.Width / 2, rectangleMeshBuildScript.Height / 2);

              _gizmoExtents = new Vector3(rectangleMeshBuildScript.Width / 2, rectangleMeshBuildScript.Height / 2);

              break;

            default:
              throw new ArgumentException("rectangleMeshBuildScript anchor " + rectangleMeshBuildScript.Anchor + " not supported.");
          }
        }
        else
        {
          var boxCollider = PlatformPrefab.GetComponent<BoxCollider2D>();

          if (boxCollider != null)
          {
            _gizmoCenter = boxCollider.offset;
            _gizmoExtents = boxCollider.bounds.extents;
          }
        }
      }

      for (var i = 0; i < PlatformPositions.Count; i++)
      {
        GizmoUtility.DrawBoundingBox(
          transform.TransformPoint(PlatformPositions[i] + _gizmoCenter),
          _gizmoExtents,
          OutlineGizmoColor);
      }
    }
  }
}

#endif
