public class IsTakingDamageController : PlayerStateController
{
  public IsTakingDamageController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.TakingDamage) == 0)
    {
      return PlayerStateUpdateResult.Unhandled;
    }
    
    return PlayerStateUpdateResult.CreateHandled("Taking Damage");
  }
}
