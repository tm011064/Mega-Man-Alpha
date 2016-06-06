#if UNITY_EDITOR
using UnityEngine;

public partial class CameraTrolley : MonoBehaviour
{
  public Color OutlineGizmoColor = Color.white;

  public bool ShowGizmoOutline = true;

  public Color ViewPortOutlineGizmoColor = Color.cyan;

  public bool ShowViewPortOutlineGizmoColor = true;

  private CameraController _cameraController;

  private Vector3 _verticalCameraBorder;

  void OnDrawGizmos()
  {
    if (_cameraController == null)
    {
      _cameraController = FindObjectOfType<CameraController>();
      _verticalCameraBorder = new Vector3(0f, _cameraController.TargetScreenSize.y * .5f, 0f);
    }

    if (ShowGizmoOutline)
    {
      for (var i = 1; i < Nodes.Count; i++)
      {
        Gizmos.color = OutlineGizmoColor;

        Gizmos.DrawLine(
          gameObject.transform.TransformPoint(Nodes[i - 1]),
          gameObject.transform.TransformPoint(Nodes[i]));

        Gizmos.color = ViewPortOutlineGizmoColor;

        Gizmos.DrawLine(
          gameObject.transform.TransformPoint(Nodes[i - 1]) + _verticalCameraBorder,
          gameObject.transform.TransformPoint(Nodes[i]) + _verticalCameraBorder);

        Gizmos.DrawLine(
          gameObject.transform.TransformPoint(Nodes[i - 1]) - _verticalCameraBorder,
          gameObject.transform.TransformPoint(Nodes[i]) - _verticalCameraBorder);
      }
    }
  }
}
#endif
