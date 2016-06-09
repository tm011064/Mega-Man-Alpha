#if UNITY_EDITOR
using UnityEngine;

public partial class SpawnBucket : BaseMonoBehaviour
{
  public Color OutlineGizmoColor = Color.yellow;

  public bool ShowGizmoOutline = true;

  private Vector3 _gizmoCenter = Vector3.zero;

  private Vector3 _gizmoExtents = new Vector3(16, 16, 0);

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      var boxCollider = GetComponent<BoxCollider2D>();

      if (boxCollider != null)
      {
        _gizmoCenter = boxCollider.offset;
        _gizmoExtents = boxCollider.size / 2;
      }

      GizmoUtility.DrawBoundingBox(transform.TransformPoint(_gizmoCenter), _gizmoExtents, OutlineGizmoColor);
    }
  }

  void Awake()
  {
    // TODO (Roman): this only works for editor, should be done on building the app as well
    // if in editor, auto register to facilitate
    RegisterChildObjects();
  }

  public void RegisterChildObjects()
  {
    _children = gameObject.GetComponentsInChildren<SpawnBucketItemBehaviour>();
  }
}
#endif
