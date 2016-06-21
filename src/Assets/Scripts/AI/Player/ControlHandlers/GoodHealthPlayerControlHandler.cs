using UnityEngine;

public class GoodHealthPlayerControlHandler : DefaultPlayerControlHandler
{
  public GoodHealthPlayerControlHandler(PlayerController playerController)
    : base(playerController)
  {
    SetDebugDraw(Color.green, true);
  }
}

