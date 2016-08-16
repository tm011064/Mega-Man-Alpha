#if UNITY_EDITOR

using UnityEngine;

public partial class FullScreenScroller : IInstantiable
{
  public Color OutlineGizmoColor = Color.yellow;

  public bool ShowGizmoOutline = true;

  public void Instantiate(InstantiationArguments arguments)
  {
    SetPosition(arguments.Bounds);

    MustBeOnLadderToEnter = arguments.GetBool("Enter On Ladder");

    transform.ForEachChildComponent<IInstantiable>(
      instantiable => instantiable.Instantiate(arguments));
  }

  private void SetPosition(Bounds bounds)
  {
    transform.position = bounds.center;

    EnableDefaultVerticalLockPosition = true;
    DefaultVerticalLockPosition = transform.position.y;

    Size = bounds.size;
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