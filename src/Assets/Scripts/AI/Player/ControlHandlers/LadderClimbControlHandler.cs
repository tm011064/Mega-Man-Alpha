using UnityEngine;

public class LadderClimbControlHandler : PlayerControlHandler
{
  private readonly Vector2 _collisionExtents;

  private readonly float _ladderTopAnimationDistance;

  private readonly Transform _transform;

  public LadderClimbControlHandler(
    PlayerController playerController,
    Transform transform,
    Vector2 collisionExtents,
    float ladderTopAnimationDistance)
    : base(playerController, new PlayerStateController[] { new ClimbController(playerController) })
  {
    SetDebugDraw(Color.green, true);

    _ladderTopAnimationDistance = ladderTopAnimationDistance;
    _collisionExtents = collisionExtents;
    _transform = transform;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsDown) != 0)
    {
      PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

      PlayerController.OnFellFromClimb();

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (GameManager.Player.EnvironmentBoxCollider.bounds.max.y
      > _transform.position.y + _collisionExtents.y + _ladderTopAnimationDistance)
    {
      GameManager.Player.InsertControlHandlerBeforeCurrent(
        new ClimbOverLadderTopControlHandler(PlayerController, _transform, _collisionExtents));

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (GameManager.Player.EnvironmentBoxCollider.bounds.max.y
      < _transform.position.y - _collisionExtents.y)
    {
      PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (GameManager.Player.IsAttacking())
    {
      return ControlHandlerAfterUpdateStatus.KeepAlive;
    }

    var yAxis = GameManager.InputStateManager.GetVerticalAxisState().Value;

    var velocity = new Vector3(
      0f,
      yAxis > 0f
        ? PlayerController.ClimbSettings.ClimbUpVelocity
        : yAxis < 0f
          ? PlayerController.ClimbSettings.ClimbDownVelocity
          : 0f);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    if (PlayerController.IsGrounded())
    {
      PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
