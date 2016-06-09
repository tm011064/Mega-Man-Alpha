#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

public partial class MultiWayCameraModifier : MonoBehaviour
{
  public ImportCameraSettings ImportCameraSettings;

  private BoxVertices _greenBox;

  private BoxVertices _redBox;

  private void SetColoredBoxVertices()
  {
    var edgeCollider = GetComponent<EdgeCollider2D>();

    var normal = new Vector2(-edgeCollider.points[1].y, edgeCollider.points[1].x).normalized;

    var boxWidth = 128f;

    _greenBox.LeftTop = edgeCollider.points[1];
    _greenBox.RightTop = edgeCollider.points[1] - normal * boxWidth;
    _greenBox.RightBottom = edgeCollider.points[0] - normal * boxWidth;
    _greenBox.LeftBottom = edgeCollider.points[0];

    _redBox.RightTop = edgeCollider.points[1];
    _redBox.LeftTop = edgeCollider.points[1] + normal * boxWidth;
    _redBox.LeftBottom = edgeCollider.points[0] + normal * boxWidth;
    _redBox.RightBottom = edgeCollider.points[0];
  }

  void OnDrawGizmosSelected()
  {
    if (GreenCameraModificationSettings.HorizontalLockSettings.Enabled
      && GreenCameraModificationSettings.HorizontalLockSettings.EnableRightHorizontalLock)
    {
      Gizmos.color = Color.green;

      var from = new Vector3(
        ParentPositionObject.transform.TransformPoint(
          GreenCameraModificationSettings.HorizontalLockSettings.RightHorizontalLockPosition, 0f, 0f).x,
          transform.position.y - 256f);

      var to = new Vector3(
        ParentPositionObject.transform.TransformPoint(
          GreenCameraModificationSettings.HorizontalLockSettings.RightHorizontalLockPosition, 0f, 0f).x,
          transform.position.y + 0f);

      Gizmos.DrawLine(from, to);
    }

    if (GreenCameraModificationSettings.HorizontalLockSettings.Enabled
      && GreenCameraModificationSettings.HorizontalLockSettings.EnableLeftHorizontalLock)
    {
      Gizmos.color = Color.green;

      var from = new Vector3(
        ParentPositionObject.transform.TransformPoint(
          GreenCameraModificationSettings.HorizontalLockSettings.LeftHorizontalLockPosition, 0f, 0f).x,
          transform.position.y - 256f);

      var to = new Vector3(
        ParentPositionObject.transform.TransformPoint(
          GreenCameraModificationSettings.HorizontalLockSettings.LeftHorizontalLockPosition, 0f, 0f).x,
          transform.position.y + 0f);

      Gizmos.DrawLine(from, to);
    }

    if (GreenCameraModificationSettings.VerticalLockSettings.Enabled
      && GreenCameraModificationSettings.VerticalLockSettings.EnableTopVerticalLock)
    {
      Gizmos.color = Color.green;

      var from = new Vector3(
        transform.position.x - 256f,
        ParentPositionObject.transform.TransformPoint(0f, GreenCameraModificationSettings.VerticalLockSettings.TopVerticalLockPosition, 0f).y);

      var to = new Vector3(
        transform.position.x + 0f,
        ParentPositionObject.transform.TransformPoint(0f, GreenCameraModificationSettings.VerticalLockSettings.TopVerticalLockPosition, 0f).y);

      Gizmos.DrawLine(from, to);
    }

    if (GreenCameraModificationSettings.VerticalLockSettings.Enabled
      && GreenCameraModificationSettings.VerticalLockSettings.EnableBottomVerticalLock)
    {
      Gizmos.color = Color.green;

      var from = new Vector3(
        transform.position.x - 256f,
        ParentPositionObject.transform.TransformPoint(0f, GreenCameraModificationSettings.VerticalLockSettings.BottomVerticalLockPosition, 0f).y);

      var to = new Vector3(
        transform.position.x + 0,
        ParentPositionObject.transform.TransformPoint(0f, GreenCameraModificationSettings.VerticalLockSettings.BottomVerticalLockPosition, 0f).y);

      Gizmos.DrawLine(from, to);
    }

    if (GreenCameraModificationSettings.VerticalLockSettings.Enabled
      && GreenCameraModificationSettings.VerticalLockSettings.EnableDefaultVerticalLockPosition)
    {
      Gizmos.color = Color.green;

      var from = new Vector3(
        transform.position.x - 512f,
        ParentPositionObject.transform.TransformPoint(0f, GreenCameraModificationSettings.VerticalLockSettings.DefaultVerticalLockPosition, 0f).y);

      var to = new Vector3(
        transform.position.x + 0f,
        ParentPositionObject.transform.TransformPoint(0f, GreenCameraModificationSettings.VerticalLockSettings.DefaultVerticalLockPosition, 0f).y);

      Gizmos.DrawLine(from, to);
    }

    if (RedCameraModificationSettings.HorizontalLockSettings.Enabled
      && RedCameraModificationSettings.HorizontalLockSettings.EnableRightHorizontalLock)
    {
      Gizmos.color = Color.red;

      var from = new Vector3(
        ParentPositionObject.transform.TransformPoint(
          RedCameraModificationSettings.HorizontalLockSettings.RightHorizontalLockPosition, 0f, 0f).x,
          transform.position.y - 0f);

      var to = new Vector3(
        ParentPositionObject.transform.TransformPoint(
          RedCameraModificationSettings.HorizontalLockSettings.RightHorizontalLockPosition, 0f, 0f).x,
          transform.position.y + 256f);

      Gizmos.DrawLine(from, to);
    }

    if (RedCameraModificationSettings.HorizontalLockSettings.Enabled
      && RedCameraModificationSettings.HorizontalLockSettings.EnableLeftHorizontalLock)
    {
      Gizmos.color = Color.red;

      var from = new Vector3(
        ParentPositionObject.transform.TransformPoint(RedCameraModificationSettings.HorizontalLockSettings.LeftHorizontalLockPosition, 0f, 0f).x,
        transform.position.y - 0f);

      var to = new Vector3(
        ParentPositionObject.transform.TransformPoint(RedCameraModificationSettings.HorizontalLockSettings.LeftHorizontalLockPosition, 0f, 0f).x,
        transform.position.y + 256f);

      Gizmos.DrawLine(from, to);
    }

    if (RedCameraModificationSettings.VerticalLockSettings.Enabled
      && RedCameraModificationSettings.VerticalLockSettings.EnableTopVerticalLock)
    {
      Gizmos.color = Color.red;

      var from = new Vector3(
        transform.position.x - 0f,
        ParentPositionObject.transform.TransformPoint(0f, RedCameraModificationSettings.VerticalLockSettings.TopVerticalLockPosition, 0f).y);

      var to = new Vector3(
        transform.position.x + 256f,
        ParentPositionObject.transform.TransformPoint(0f, RedCameraModificationSettings.VerticalLockSettings.TopVerticalLockPosition, 0f).y);

      Gizmos.DrawLine(from, to);
    }

    if (RedCameraModificationSettings.VerticalLockSettings.Enabled
      && RedCameraModificationSettings.VerticalLockSettings.EnableBottomVerticalLock)
    {
      Gizmos.color = Color.red;

      var from = new Vector3(
        transform.position.x - 0f,
        ParentPositionObject.transform.TransformPoint(0f, RedCameraModificationSettings.VerticalLockSettings.BottomVerticalLockPosition, 0f).y);

      var to = new Vector3(
        transform.position.x + 256f,
        ParentPositionObject.transform.TransformPoint(0f, RedCameraModificationSettings.VerticalLockSettings.BottomVerticalLockPosition, 0f).y);

      Gizmos.DrawLine(from, to);
    }

    if (RedCameraModificationSettings.VerticalLockSettings.Enabled
      && RedCameraModificationSettings.VerticalLockSettings.EnableDefaultVerticalLockPosition)
    {
      Gizmos.color = Color.red;

      var from = new Vector3(
        transform.position.x - 0f,
        ParentPositionObject.transform.TransformPoint(0f, RedCameraModificationSettings.VerticalLockSettings.DefaultVerticalLockPosition, 0f).y);

      var to = new Vector3(
        transform.position.x + 512f,
        ParentPositionObject.transform.TransformPoint(0f, RedCameraModificationSettings.VerticalLockSettings.DefaultVerticalLockPosition, 0f).y);

      Gizmos.DrawLine(from, to);
    }
  }

  void OnDrawGizmos()
  {
    SetColoredBoxVertices();

    Gizmos.color = Color.green;
    Gizmos.DrawLine(transform.TransformPoint(_greenBox.LeftTop), transform.TransformPoint(_greenBox.RightTop));
    Gizmos.DrawLine(transform.TransformPoint(_greenBox.RightTop), transform.TransformPoint(_greenBox.RightBottom));
    Gizmos.DrawLine(transform.TransformPoint(_greenBox.RightBottom), transform.TransformPoint(_greenBox.LeftBottom));

    Gizmos.color = Color.red;
    Gizmos.DrawLine(transform.TransformPoint(_redBox.RightTop), transform.TransformPoint(_redBox.LeftTop));
    Gizmos.DrawLine(transform.TransformPoint(_redBox.LeftTop), transform.TransformPoint(_redBox.LeftBottom));
    Gizmos.DrawLine(transform.TransformPoint(_redBox.LeftBottom), transform.TransformPoint(_redBox.RightBottom));

    Gizmos.color = Color.white;
    Gizmos.DrawLine(transform.TransformPoint(_greenBox.LeftBottom), transform.TransformPoint(_greenBox.LeftTop));
  }

  public void ImportSettings()
  {
    if (ImportCameraSettings.ImportSource == null)
    {
      Debug.LogWarning("Unable to import settings because no object was selected");

      return;
    }

    var multiWayCameraModifier = ImportCameraSettings.ImportSource.GetComponent<MultiWayCameraModifier>();

    var cameraModifier = ImportCameraSettings.ImportSource.GetComponent<CameraModifier>();

    if (multiWayCameraModifier != null)
    {
      switch (ImportCameraSettings.ImportCameraSettingsMode)
      {
        case ImportCameraSettingsMode.FromGreenToGreen:

          GreenCameraModificationSettings = multiWayCameraModifier.GreenCameraModificationSettings.Clone();

          Debug.Log("Successfully imported green settings from " + ImportCameraSettings.ImportSource.name + " and applied them to greenCameraModificationSettings");

          break;

        case ImportCameraSettingsMode.FromGreenToRed:

          RedCameraModificationSettings = multiWayCameraModifier.GreenCameraModificationSettings.Clone();

          Debug.Log("Successfully imported green settings from " + ImportCameraSettings.ImportSource.name + " and applied them to redCameraModificationSettings");

          break;

        case ImportCameraSettingsMode.FromRedToGreen:

          GreenCameraModificationSettings = multiWayCameraModifier.RedCameraModificationSettings.Clone();

          Debug.Log("Successfully imported red settings from " + ImportCameraSettings.ImportSource.name + " and applied them to greenCameraModificationSettings");

          break;

        case ImportCameraSettingsMode.FromRedToRed:

          RedCameraModificationSettings = multiWayCameraModifier.RedCameraModificationSettings.Clone();

          Debug.Log("Successfully imported red settings from " + ImportCameraSettings.ImportSource.name + " and applied them to redCameraModificationSettings");

          break;

        case ImportCameraSettingsMode.FromModifierToGreen:
        case ImportCameraSettingsMode.FromModifierToRed:

          Debug.LogError("Unable to import modifer settings because the source is of type MultiWayCameraModifier");

          break;
      }
    }
    else if (cameraModifier != null)
    {
      var multiWayCameraModificationSetting = new MultiWayCameraModificationSetting();

      multiWayCameraModificationSetting.VerticalLockSettings = cameraModifier.VerticalLockSettings.Clone();
      multiWayCameraModificationSetting.HorizontalLockSettings = cameraModifier.HorizontalLockSettings.Clone();
      multiWayCameraModificationSetting.ZoomSettings = cameraModifier.ZoomSettings.Clone();
      multiWayCameraModificationSetting.SmoothDampMoveSettings = cameraModifier.SmoothDampMoveSettings.Clone();

      multiWayCameraModificationSetting.Offset = cameraModifier.Offset;
      multiWayCameraModificationSetting.VerticalCameraFollowMode = cameraModifier.VerticalCameraFollowMode;
      multiWayCameraModificationSetting.HorizontalOffsetDeltaMovementFactor = cameraModifier.HorizontalOffsetDeltaMovementFactor;

      switch (ImportCameraSettings.ImportCameraSettingsMode)
      {
        case ImportCameraSettingsMode.FromGreenToGreen:
        case ImportCameraSettingsMode.FromRedToGreen:
        case ImportCameraSettingsMode.FromModifierToGreen:

          GreenCameraModificationSettings = multiWayCameraModificationSetting;

          Debug.Log("Successfully imported settings from " + ImportCameraSettings.ImportSource.name + " and applied them to greenCameraModificationSettings");

          break;

        case ImportCameraSettingsMode.FromGreenToRed:
        case ImportCameraSettingsMode.FromRedToRed:
        case ImportCameraSettingsMode.FromModifierToRed:

          RedCameraModificationSettings = multiWayCameraModificationSetting;

          Debug.Log("Successfully imported settings from " + ImportCameraSettings.ImportSource.name + " and applied them to redCameraModificationSettings");

          break;
      }
    }
    else
    {
      Debug.LogError("Unable to import settings because object does not contain MultiWayCameraModifier nor CameraModifier component.");

      return;
    }
  }

  public void BuildObject()
  {
    var edgeCollider = GetComponent<EdgeCollider2D>();

    edgeCollider.hideFlags = HideFlags.NotEditable;

    var points = new List<Vector2>() { Vector2.zero };

    var x = Mathf.Sin(EdgeColliderAngle * Mathf.Deg2Rad) * EdgeColliderLength;
    var y = Mathf.Cos(EdgeColliderAngle * Mathf.Deg2Rad) * EdgeColliderLength;

    points.Add(new Vector2(x, y));

    edgeCollider.points = points.ToArray();

    SetColoredBoxVertices();
  }

  private struct BoxVertices
  {
    public Vector2 LeftTop;

    public Vector2 RightTop;

    public Vector2 LeftBottom;

    public Vector2 RightBottom;
  }
}
#endif
