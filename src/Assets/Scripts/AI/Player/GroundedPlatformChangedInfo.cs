using UnityEngine;

public class GroundedPlatformChangedInfo
{
  public GameObject PreviousPlatform;

  public GameObject CurrentPlatform;

  public GroundedPlatformChangedInfo(GameObject previousPlatform, GameObject currentPlatform)
  {
    PreviousPlatform = previousPlatform;
    CurrentPlatform = currentPlatform;
  }
}