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

    VerticalCameraFollowMode = VerticalCameraFollowMode.FollowAlways;

    foreach (var args in arguments.BoundsPropertyInfos)
    {
      var boxColliderGameObject = new GameObject("Box Collider With Enter Trigger");

      boxColliderGameObject.transform.position = args.Bounds.center;
      boxColliderGameObject.layer = gameObject.layer;
      boxColliderGameObject.transform.parent = gameObject.transform;

      var boxCollider = boxColliderGameObject.AddComponent<BoxCollider2D>();

      boxCollider.isTrigger = true;
      boxCollider.size = args.Bounds.size;

      var boxColliderTriggerEnterBehaviour = boxColliderGameObject.AddComponent<BoxColliderTriggerEnterBehaviour>();

      if (args.Properties.GetBool("Enter On Ladder"))
      {
        boxColliderTriggerEnterBehaviour.PlayerStatesNeededToEnter = new PlayerState[] { PlayerState.ClimbingLadder };
      }
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
