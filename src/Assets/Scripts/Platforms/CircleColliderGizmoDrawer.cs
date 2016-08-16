using UnityEngine;

public class CircleColliderGizmoDrawer : BaseMonoBehaviour
{
  public Color OutlineGizmoColor = Color.white;

  public Color OutlineVisibilityMaskGizmoColor = Color.magenta;

  public bool ShowGizmoOutline = true;

  public bool ShowCameraGizmoOutline = true;

#if UNITY_EDITOR
  private CircleCollider2D _circleCollider = null;

  void OnDrawGizmos()
  {
    if (_circleCollider == null)
    {
      _circleCollider = this.GetComponentOrThrow<CircleCollider2D>();
    }

    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, _circleCollider.radius);
  }
#endif

}