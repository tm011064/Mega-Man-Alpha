#if UNITY_EDITOR

using System;
using UnityEngine;

public partial class JumpControlledPlatformSwitchGroup : SpawnBucketItemBehaviour
{
  void OnDrawGizmos()
  {
    for (var i = 0; i < PlatformGroupPositions.Count; i++)
    {
      if (PlatformGroupPositions[i].ShowGizmoOutline)
      {
        var gizmoCenter = Vector3.zero;

        var gizmoExtents = new Vector3(16, 16, 0);

        if (PlatformGroupPositions[i].EnabledGameObject != null)
        {
          for (int j = 0; j < PlatformGroupPositions[i].Positions.Count; j++)
          {
            var rectangleMeshBuildScript =
              PlatformGroupPositions[i].EnabledGameObject.GetComponent<RectangleMeshBuildScript>();

            if (rectangleMeshBuildScript != null)
            {
              switch (rectangleMeshBuildScript.Anchor)
              {
                case TextAnchor.MiddleCenter:

                  gizmoCenter = Vector3.zero;

                  gizmoExtents = new Vector3(rectangleMeshBuildScript.Width / 2, rectangleMeshBuildScript.Height / 2);

                  break;

                case TextAnchor.LowerLeft:

                  gizmoCenter = new Vector3(rectangleMeshBuildScript.Width / 2, rectangleMeshBuildScript.Height / 2);

                  gizmoExtents = new Vector3(rectangleMeshBuildScript.Width / 2, rectangleMeshBuildScript.Height / 2);

                  break;

                default:
                  throw new ArgumentException("rectangleMeshBuildScript anchor " + rectangleMeshBuildScript.Anchor + " not supported.");
              }
            }
            else
            {
              var boxCollider = PlatformGroupPositions[i].EnabledGameObject.GetComponent<BoxCollider2D>();

              if (boxCollider != null)
              {
                gizmoCenter = boxCollider.offset;
                gizmoExtents = boxCollider.size / 2;
              }
            }

            GizmoUtility.DrawBoundingBox(transform.TransformPoint(PlatformGroupPositions[i].Positions[j] + gizmoCenter), gizmoExtents, PlatformGroupPositions[i].OutlineGizmoColor);
          }
        }
      }
    }
  }
}

#endif