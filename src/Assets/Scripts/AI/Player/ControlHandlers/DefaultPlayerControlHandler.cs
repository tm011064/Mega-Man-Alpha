﻿using UnityEngine;

public class DefaultPlayerControlHandler : PlayerControlHandler
{
  public DefaultPlayerControlHandler(PlayerController playerController)
    : base(playerController)
  {
  }

  public DefaultPlayerControlHandler(
    PlayerController playerController,
    PlayerStateController[] animationStateControllers,
    float duration)
    : base(playerController, animationStateControllers, duration)
  {
  }

  private bool DoStartSlide()
  {
    return PlayerController.SlideSettings.EnableSliding
      && PlayerController.IsGrounded()
      && GameManager.InputStateManager.AreButtonsPressed(
        PlayerController.SlideSettings.InputButtonNames,
        PlayerController.InputSettings);
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    HandleOneWayPlatformFallThrough();

    if (DoStartSlide())
    {
      PlayerController.InsertControlHandlerBeforeCurrent(Clone());
      PlayerController.InsertControlHandlerBeforeCurrent(new SlidePlayerControlHandler(PlayerController));

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    velocity.y = GetJumpVerticalVelocity(velocity);

    velocity.y = Mathf.Max(
      GetGravityAdjustedVerticalVelocity(velocity, PlayerController.AdjustedGravity, true),
      PlayerController.JumpSettings.MaxDownwardSpeed);

    velocity.x = GetDefaultHorizontalVelocity(velocity);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
