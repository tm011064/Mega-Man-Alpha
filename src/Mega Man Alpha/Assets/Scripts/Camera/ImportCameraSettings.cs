#if UNITY_EDITOR

using System;
using UnityEngine;

[Serializable]
public class ImportCameraSettings
{
  [Tooltip("Must contain MultiWayCameraModifier or CameraModifier component")]
  public GameObject ImportSource;

  public ImportCameraSettingsMode ImportCameraSettingsMode;
}

#endif
