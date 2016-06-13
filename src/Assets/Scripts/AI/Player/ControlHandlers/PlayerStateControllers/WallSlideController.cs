using System;
using UnityEngine;

public class WallSlideController : PlayerStateController
{
  public WallSlideController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override void UpdateState(XYAxisState axisState)
  {
    if (
        (PlayerController.PlayerState & PlayerState.AttachedToWall) != 0
      && PlayerController.CharacterPhysicsManager.Velocity.y < 0f
      &&
      (
        (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.CharacterWallState & CharacterWallState.OnRightWall) != 0
        || (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.CharacterWallState & CharacterWallState.OnLeftWall) != 0
      ))
    {
      // TODO (Roman): this should be done via mecanim
      PlayerController.CharacterPhysicsManager.BoxCollider.offset = PlayerController.BoxColliderOffsetWallAttached;
      PlayerController.CharacterPhysicsManager.BoxCollider.size = PlayerController.BoxColliderSizeWallAttached;
    }
  }

  public override AnimationPlayResult PlayAnimation(XYAxisState axisState)
  {
    if (
      IsSlidingDownWall(
        () => (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.CharacterWallState & CharacterWallState.OnRightWall) != 0,
        () => PlayerController.Sprite.transform.localScale.x < 1f)
      ||
      IsSlidingDownWall(
        () => (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.CharacterWallState & CharacterWallState.OnLeftWall) != 0,
        () => PlayerController.Sprite.transform.localScale.x > -1f))
    {
      PlayerController.Animator.Play(Animator.StringToHash("PlayerWallAttached"));

      return AnimationPlayResult.Played;
    }

    return AnimationPlayResult.NotPlayed;
  }

  private bool IsSlidingDownWall(Func<bool> isOnWall, Func<bool> flipScaleX)
  {
    if ((PlayerController.PlayerState & PlayerState.AttachedToWall) == 0
      || PlayerController.CharacterPhysicsManager.Velocity.y >= 0f
      || !isOnWall())
    {
      return false;
    }

    if (flipScaleX())
    {
      PlayerController.Sprite.transform.localScale =
        new Vector3(
          PlayerController.Sprite.transform.localScale.x * -1,
          PlayerController.Sprite.transform.localScale.y,
          PlayerController.Sprite.transform.localScale.z);
    }

    return true;
  }
}
