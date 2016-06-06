﻿using UnityEngine;

public class SlideDownSlopePlayerControlHandler : PlayerControlHandler
{
  private Direction _platformDirection;

  public SlideDownSlopePlayerControlHandler(PlayerController playerController, Direction platformDirection)
    : base(playerController)
  {
    _platformDirection = platformDirection;
  }

  public SlideDownSlopePlayerControlHandler(PlayerController playerController, float duration, Direction platformDirection)
    : base(playerController, duration)
  {
    _platformDirection = platformDirection;
  }

  protected override bool DoUpdate()
  {
    CheckOneWayPlatformFallThrough();

    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    var yVelocity = Mathf.Max(
      GetGravityAdjustedVerticalVelocity(velocity, PlayerController.AdjustedGravity, true),
      PlayerController.JumpSettings.MaxDownwardSpeed);

    var moveCalculationResult =
      PlayerController.CharacterPhysicsManager.SlideDown(_platformDirection, yVelocity * Time.deltaTime);

    if (_platformDirection == Direction.Left && !moveCalculationResult.CollisionState.Left)
    {
      return false;
    }
    if (_platformDirection == Direction.Right && !moveCalculationResult.CollisionState.Right)
    {
      return false;
    }

    Logger.Trace("PlayerMetricsDebug", "Position: " + PlayerController.transform.position + ", Velocity: " + velocity);

    return true;
  }
}

