using UnityEngine;

public class BadHealthPlayerControlHandler : DefaultPlayerControlHandler
{
  public BadHealthPlayerControlHandler(PlayerController playerController)
    : base(playerController)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.red;
  }
}
