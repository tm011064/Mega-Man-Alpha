using UnityEngine;

public class MovingPlatformSwitch : MonoBehaviour
{
  private DynamicPingPongPath[] _dynamicPingPongPaths = null;

  void OnEnable()
  {
    if (_dynamicPingPongPaths == null)
    {
      _dynamicPingPongPaths = gameObject.GetComponentsInChildren<DynamicPingPongPath>(true);
    }
  }

  void OnTriggerExit2D(Collider2D col)
  {
    for (var i = 0; i < _dynamicPingPongPaths.Length; i++)
    {
      _dynamicPingPongPaths[i].StopForwardMovement();
    }
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    for (var i = 0; i < _dynamicPingPongPaths.Length; i++)
    {
      _dynamicPingPongPaths[i].StartForwardMovement();
    }
  }
}
