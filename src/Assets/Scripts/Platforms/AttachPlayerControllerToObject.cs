using System;
using UnityEngine;

public class AttachPlayerControllerToObject : MonoBehaviour, IAttachableObject
{
  protected PlayerController _playerController;

  public event Action PlayerControllerGotGrounded;

  public event Action<IAttachableObject, GameObject> Attached;

  public event Action<IAttachableObject, GameObject> Detached;

  void OnPlayerGroundedPlatformChanged(GroundedPlatformChangedInfo e)
  {
    if (e.PreviousPlatform == gameObject 
      && _playerController.transform.parent == gameObject.transform)
    {
      _playerController.transform.parent = null;

      var handler = Detached;

      if (handler != null)
      {
        handler.Invoke(this, gameObject);
      }

      Logger.Info("Removed parent (" + gameObject.name + " [ " + GetHashCode() + " ]) relationship from child (" + _playerController.name + ") [1]");
    }
    else
    {
      if (e.CurrentPlatform == gameObject
        && _playerController.transform.parent != gameObject.transform)
      {
        if (_playerController.transform.parent != null)
        {
          _playerController.transform.parent = null;

          Logger.Info("Removed parent (" + gameObject.name + " [ " + GetHashCode() + " ]) relationship from child (" + _playerController.name + ") [2]");
        }

        _playerController.transform.parent = gameObject.transform;

        Logger.Info("Added parent (" + gameObject.name + " [ " + GetHashCode() + " ]) relationship to child (" + _playerController.name + ")");

        var playerControllerGotGroundedHandler = PlayerControllerGotGrounded;

        if (playerControllerGotGroundedHandler != null)
        {
          playerControllerGotGroundedHandler.Invoke();
        }

        var attachedHandler = Attached;

        if (attachedHandler != null)
        {
          attachedHandler.Invoke(this, gameObject);
        }
      }
    }
  }

  void OnBeforeDisable()
  {
    if (_playerController != null 
      && _playerController.transform.parent == gameObject.transform)
    {
      _playerController.transform.parent = null;
    }
  }

  void OnEnable()
  {
    _playerController = GameManager.Instance.Player;

    _playerController.GroundedPlatformChanged += OnPlayerGroundedPlatformChanged;
  }

  void OnDisable()
  {
    _playerController.GroundedPlatformChanged -= OnPlayerGroundedPlatformChanged;
  }
}
