#if UNITY_EDITOR

using UnityEngine;

public partial class FullScreenScroller : IInstantiable<CameraModifierInstantiationArguments>, IInstantiable<InstantiationArguments>
{
  public Color OutlineGizmoColor = Color.yellow;

  public bool ShowGizmoOutline = true;

  public void Instantiate(InstantiationArguments arguments)
  {
    SetPosition(arguments.Bounds);

    transform.ForEachChildComponent<IInstantiable<InstantiationArguments>>(
      instantiable => instantiable.Instantiate(arguments));
  }

  public void Instantiate(CameraModifierInstantiationArguments arguments)
  {
    SetPosition(arguments.Bounds);

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

  private void SetPosition(Bounds bounds)
  {
    transform.position = bounds.center;

    Size = bounds.size;
    VerticalCameraFollowMode = VerticalCameraFollowMode.FollowAlways;
  }

  public bool Contains(Vector2 point)
  {
    var bounds = new Bounds(transform.position, Size);

    return bounds.Contains(point);
  }

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      GizmoUtility.DrawBoundingBox(transform.position, Size / 2, OutlineGizmoColor);
    }
  }
}

#endif