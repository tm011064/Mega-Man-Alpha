using UnityEngine;

public class LadderClimbControlHandler : PlayerControlHandler
{
  private readonly Bounds _ladderArea;

  private readonly float _climbOverTopExtension;

  public LadderClimbControlHandler(PlayerController playerController, Bounds ladderArea, float climbOverTopExtension)
    : base(playerController)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.green;

    _ladderArea = ladderArea;
    _climbOverTopExtension = climbOverTopExtension;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsDown) != 0)
    {
      PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

      PlayerController.OnFellFromClimb();

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (GameManager.Player.BoxCollider.bounds.AreOutsideTopHorizontalBoundsOf(_ladderArea))
    {
      GameManager.Player.InsertControlHandlerBeforeCurrent(
        new ClimbOverLadderApexControlHandler(PlayerController, _climbOverTopExtension));

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (GameManager.Player.BoxCollider.bounds.AreOutsideBottomHorizontalBoundsOf(_ladderArea))
    {
      PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var yAxis = GameManager.InputStateManager.GetAxisState("Vertical").Value;

    var velocity = new Vector3(
      0f,
      yAxis > 0f
        ? PlayerController.ClimbSettings.ClimbUpVelocity
        : yAxis < 0f
          ? PlayerController.ClimbSettings.ClimbDownVelocity
          : 0f);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    if (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
    {
      PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
