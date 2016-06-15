using UnityEngine;

public class LadderClimbControlHandler : PlayerControlHandler
{
  private readonly Bounds _ladderArea;

  private readonly float _topEdgePositionY;

  public LadderClimbControlHandler(PlayerController playerController, Bounds ladderArea, float topEdgePositionY)
    : base(playerController)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.green;

    _ladderArea = ladderArea;
    _topEdgePositionY = topEdgePositionY;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsDown) != 0)
    {
      PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

      PlayerController.OnFellFromClimb();

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (GameManager.Player.BoxCollider.bounds.AreAbove(_ladderArea))
    {
      GameManager.Player.InsertControlHandlerBeforeCurrent(
        new ClimbOverLadderTopControlHandler(PlayerController, _topEdgePositionY));

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (GameManager.Player.BoxCollider.bounds.AreBelow(_ladderArea))
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
