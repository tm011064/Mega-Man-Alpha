#if UNITY_EDITOR

using UnityEngine;

public partial class EdgeColliderTriggerEnterBehaviour : IInstantiable<CameraModifierInstantiationArguments>
{
  public void Instantiate(CameraModifierInstantiationArguments arguments)
  {
    foreach (var propertyInfo in arguments.Line2PropertyInfos)
    {
      var edgeCollider = gameObject.AddComponent<EdgeCollider2D>();

      edgeCollider.isTrigger = true;
      edgeCollider.points = propertyInfo.Line.ToVectors();

      // TODO (Roman): this must be split up, full screen scrollers must have multiple enter behaviours with
      // one edge collider each
      if (arguments.Properties.GetBoolSafe("Enter On Ladder", false))
      {
        PlayerStatesNeededToEnter = new PlayerState[] { PlayerState.ClimbingLadder };
      }
    }

    transform.position = arguments.Bounds.center;
  }
}

#endif
