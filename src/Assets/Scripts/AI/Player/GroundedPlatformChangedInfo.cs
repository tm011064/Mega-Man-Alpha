using UnityEngine;

public class GroundedPlatformChangedInfo
{
  public GameObject previousPlatform;

  public GameObject currentPlatform;

  public GroundedPlatformChangedInfo(GameObject previousPlatform, GameObject currentPlatform)
  {
    this.previousPlatform = previousPlatform;
    this.currentPlatform = currentPlatform;
  }
}