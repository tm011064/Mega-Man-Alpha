#if UNITY_EDITOR
using UnityEngine;

public partial class CameraModifier : IInstantiable<CameraModifierInstantiationArguments>
{
  public ImportCameraSettings ImportCameraSettings;

  public void Instantiate(CameraModifierInstantiationArguments arguments)
  {
    var cameraController = Camera.main.GetComponentOrThrow<CameraController>();

    VerticalLockSettings = CreateVerticalLockSettings(arguments.Bounds, cameraController);
    HorizontalLockSettings = CreateHorizontalLockSettings(arguments.Bounds, cameraController);

    foreach (var args in arguments.Line2PropertyInfos)
    {
      var edgeColliderGameObject = new GameObject("Edge Collider With Enter Trigger");

      var edgeCollider = edgeColliderGameObject.AddComponent<EdgeCollider2D>();

      edgeCollider.isTrigger = true;
      edgeCollider.points = args.Line.ToVectors();

      var edgeColliderTriggerEnterBehaviour = edgeColliderGameObject.AddComponent<EdgeColliderTriggerEnterBehaviour>();

      if (args.Properties.GetBool("Enter On Ladder"))
      {
        edgeColliderTriggerEnterBehaviour.PlayerStatesNeededToEnter = new PlayerState[] { PlayerState.ClimbingLadder };
      }

      edgeColliderGameObject.layer = gameObject.layer;

      edgeColliderGameObject.transform.parent = gameObject.transform;
    }
  }

  private VerticalLockSettings CreateVerticalLockSettings(Bounds bounds, CameraController cameraController)
  {
    var verticalLockSettings = new VerticalLockSettings
    {
      Enabled = true,
      EnableDefaultVerticalLockPosition = false,
      DefaultVerticalLockPosition = 0f,
      EnableTopVerticalLock = true,
      EnableBottomVerticalLock = true,
      TopVerticalLockPosition = bounds.max.y,
      BottomVerticalLockPosition = bounds.min.y
    };

    SetVerticalBoundaries(verticalLockSettings, cameraController);

    return verticalLockSettings;
  }
  
  private HorizontalLockSettings CreateHorizontalLockSettings(Bounds bounds, CameraController cameraController)
  {
    var horizontalLockSettings = new HorizontalLockSettings
    {
      Enabled = true,
      EnableLeftHorizontalLock = true,
      EnableRightHorizontalLock = true,
      LeftHorizontalLockPosition = bounds.min.x,
      RightHorizontalLockPosition = bounds.max.x
    };

    SetHorizontalBoundaries(horizontalLockSettings, cameraController);

    return horizontalLockSettings;
  }

  public bool Contains(Vector2 point)
  {
    var cameraMovementSettings = new CameraMovementSettings(
      VerticalLockSettings,
      HorizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      Offset,
      VerticalCameraFollowMode,
      HorizontalOffsetDeltaMovementFactor);

    return cameraMovementSettings.Contains(point);
  }

  void OnDrawGizmos()
  {
    foreach (var collider in GetComponents<EdgeCollider2D>())
    {
      Gizmos.color = GizmoColor;
      Gizmos.DrawLine(collider.points[0], collider.points[1]);

      break;
    }
  }
}
#endif
