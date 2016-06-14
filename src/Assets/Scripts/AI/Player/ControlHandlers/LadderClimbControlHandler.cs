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

  protected override bool DoUpdate()
  {
    if ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsUp) != 0)
    {
      PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

      return false;
    }

    if (GameManager.Player.BoxCollider.bounds.AreOutsideTopHorizontalBoundsOf(_ladderArea))
    {
      GameManager.Player.InsertControlHandlerBeneathCurrent(
        new ClimbOverLadderApexControlHandler(PlayerController, _climbOverTopExtension));

      return false;
    }

    if (GameManager.Player.BoxCollider.bounds.AreOutsideBottomHorizontalBoundsOf(_ladderArea))
    {
      PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

      return false;
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

    return true;
  }
}
