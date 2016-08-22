#if UNITY_EDITOR
using UnityEngine;

public partial class CameraModifier : IInstantiable
{
  public ImportCameraSettings ImportCameraSettings;

  public void Instantiate(InstantiationArguments arguments)
  {
    var cameraController = Camera.main.GetComponentOrThrow<CameraController>();

    VerticalLockSettings = CreateVerticalLockSettings(arguments.Bounds, cameraController);
    HorizontalLockSettings = CreateHorizontalLockSettings(arguments.Bounds, cameraController);

    foreach (var line in arguments.Lines)
    {
      var edgeColliderGameObject = new GameObject("Edge Collider With Enter Trigger");

      var edgeCollider = edgeColliderGameObject.AddComponent<EdgeCollider2D>();

      edgeCollider.isTrigger = true;
      edgeCollider.points = line.ToVectors();

      edgeColliderGameObject.AddComponent<EdgeColliderTriggerEnterBehaviour>();

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

    verticalLockSettings.TopBoundary =
      verticalLockSettings.TopVerticalLockPosition
      - cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.BottomBoundary =
      verticalLockSettings.BottomVerticalLockPosition
      + cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.TranslatedVerticalLockPosition =
      verticalLockSettings.DefaultVerticalLockPosition;

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

    horizontalLockSettings.LeftBoundary =
      horizontalLockSettings.LeftHorizontalLockPosition
      + cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    horizontalLockSettings.RightBoundary =
      horizontalLockSettings.RightHorizontalLockPosition
      - cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    return horizontalLockSettings;
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
