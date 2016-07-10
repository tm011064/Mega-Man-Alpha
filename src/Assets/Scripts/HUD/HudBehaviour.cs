using UnityEngine;

public class HudBehaviour : MonoBehaviour
{
  void Awake()
  {
    var cameraController = Camera.main.GetComponent<CameraController>();

    transform.position = new Vector3(
      transform.position.x,
      transform.position.y,
      cameraController.CameraOffset.z);
  }
}
