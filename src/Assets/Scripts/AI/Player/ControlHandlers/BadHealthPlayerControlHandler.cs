using UnityEngine;

public class BadHealthPlayerControlHandler : DefaultPlayerControlHandler
{
  public BadHealthPlayerControlHandler(PlayerController playerController)
    : base(playerController)
  {
    SetDebugDraw(Color.red, true);
  }
}
