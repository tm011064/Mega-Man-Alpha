using UnityEngine;

public class LadderClimbControlHandler : PlayerControlHandler
{
  private readonly Bounds _ladderArea;

  private readonly float _ladderTopEdgePosY;

  public LadderClimbControlHandler(PlayerController playerController, Bounds ladderArea, float ladderTopEdgePosY)
    : base(playerController, new PlayerStateController[] { new ClimbController(playerController) })
  {
    SetDebugDraw(Color.green, true);

    _ladderArea = ladderArea;
    _ladderTopEdgePosY = ladderTopEdgePosY;
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
        new ClimbOverLadderTopControlHandler(PlayerController, _ladderTopEdgePosY));

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (GameManager.Player.BoxCollider.bounds.AreBelow(_ladderArea))
    {
      PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
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
