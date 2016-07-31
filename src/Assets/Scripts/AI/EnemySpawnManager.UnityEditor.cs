#if UNITY_EDITOR
using UnityEngine;

public partial class EnemySpawnManager : SpawnBucketItemBehaviour, IInstantiable
{
  public Color OutlineGizmoColor = Color.yellow;

  public bool ShowGizmoOutline = true;

  private Vector3 _gizmoCenter = Vector3.zero;

  private Vector3 _gizmoExtents = new Vector3(16, 16, 0);

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      if (_enemyToSpawnPrefab != null)
      {
        var boxCollider2D = _enemyToSpawnPrefab.GetComponent<BoxCollider2D>();

        if (boxCollider2D != null)
        {
          _gizmoCenter = boxCollider2D.offset;

          _gizmoExtents = boxCollider2D.size / 2;
        }
      }

      GizmoUtility.DrawBoundingBox(transform.TransformPoint(_gizmoCenter), _gizmoExtents, OutlineGizmoColor);
    }
  }

  public void Instantiate(InstantiationArguments arguments)
  {
    transform.position = arguments.Bounds.center;

    // note: for some reason GetComponent(s)InChildren<T>() crashes here
    for (var i = 0; i < transform.childCount; i++)
    {
      var child = transform.GetChild(i);

      var instantiable = child.GetComponent<IInstantiable>();

      if (instantiable != null)
      {
        instantiable.Instantiate(arguments);
      }
    }
  }
}
#endif