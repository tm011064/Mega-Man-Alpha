using UnityEngine;

public class GoodHealthPlayerControlHandler : DefaultPlayerControlHandler
{
  public GoodHealthPlayerControlHandler(PlayerController playerController)
    : base(playerController)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.green;
  }
}

