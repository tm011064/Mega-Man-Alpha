using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
  public float Speed = 200f;

  protected PlayerController _playerController;

  private bool _isGrounded;

  void Update()
  {
    if (_isGrounded)
    {
      _playerController.CharacterPhysicsManager.AddHorizontalForce(Speed * Time.deltaTime);
    }
  }

  void OnPlayerGroundedPlatformChanged(GroundedPlatformChangedInfo e)
  {
    if (e.CurrentPlatform == gameObject)
    {
      _isGrounded = true;
    }
    else
    {
      _isGrounded = false;
    }
  }

  void OnEnable()
  {
    _playerController.GroundedPlatformChanged += OnPlayerGroundedPlatformChanged;
  }

  void OnDisable()
  {
    _playerController.GroundedPlatformChanged -= OnPlayerGroundedPlatformChanged;
  }

  void Awake()
  {
    _playerController = GameManager.Instance.Player;
  }
}
