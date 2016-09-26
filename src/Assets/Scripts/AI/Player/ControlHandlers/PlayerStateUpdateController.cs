using UnityEngine;

public class PlayerStateUpdateController
{
  private readonly PlayerController _playerController;

  private readonly PlayerStateController[] _playerStateControllers;

  public PlayerStateUpdateController(
    PlayerController playerController,
    PlayerStateController[] playerStateControllers)
  {
    _playerController = playerController;
    _playerStateControllers = playerStateControllers;
  }
  
  public void UpdatePlayerState(XYAxisState axisState)
  {
    var playerStateUpdateResult = PlayerStateUpdateResult.Max(
        UpdatePlayerStateControllers(axisState),
        UpdateWeaponControllers(axisState));

    AdjustSpriteScale(axisState);

    if (playerStateUpdateResult.AnimationClipInfo != null)
    {
      PlayAnimation(playerStateUpdateResult.AnimationClipInfo);
    }
  }

  private PlayerStateUpdateResult UpdateWeaponControllers(XYAxisState axisState)
  {
    foreach (var weaponControlHandler in _playerController.WeaponControlHandlers)
    {
      var playerStateUpdateResult = weaponControlHandler.Update(axisState);

      if (playerStateUpdateResult.IsHandled)
      {
        return playerStateUpdateResult;
      }
    }

    return PlayerStateUpdateResult.Unhandled;
  }

  private PlayerStateUpdateResult UpdatePlayerStateControllers(XYAxisState axisState)
  {
    for (var i = 0; i < _playerStateControllers.Length; i++)
    {
      var playerStateUpdateResult = _playerStateControllers[i].UpdatePlayerState(axisState);

      if (playerStateUpdateResult.IsHandled)
      {
        return playerStateUpdateResult;
      }
    }

    return PlayerStateUpdateResult.Unhandled;
  }

  private void AdjustSpriteScale(XYAxisState axisState)
  {
    if ((axisState.XAxis > 0f && _playerController.Sprite.transform.localScale.x < 1f)
      || (axisState.XAxis < 0f && _playerController.Sprite.transform.localScale.x > -1f))
    {
      _playerController.Sprite.transform.localScale = new Vector3(
        _playerController.Sprite.transform.localScale.x * -1,
        _playerController.Sprite.transform.localScale.y,
        _playerController.Sprite.transform.localScale.z);
    }
  }

  private void PlayAnimation(AnimationClipInfo animationClipInfo)
  {
    _playerController.Animator.speed = animationClipInfo.Speed;

    var animatorStateInfo = _playerController.Animator.GetCurrentAnimatorStateInfo(0);

    if (animatorStateInfo.shortNameHash == animationClipInfo.ShortNameHash)
    {
      return;
    }

    if (animationClipInfo.LinkedShortNameHashes != null)
    {
      foreach (var hash in animationClipInfo.LinkedShortNameHashes)
      {
        if (animatorStateInfo.shortNameHash == hash)
        {
          return;
        }
      }
    }

    _playerController.Animator.Play(animationClipInfo.ShortNameHash);
  }
}