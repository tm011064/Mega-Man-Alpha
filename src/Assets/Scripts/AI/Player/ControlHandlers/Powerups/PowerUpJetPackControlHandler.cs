using UnityEngine;

public class PowerUpJetPackControlHandler : PlayerControlHandler
{
  private PowerUpSettings _powerUpSettings;

  private float _lastBulletTime;

  public PowerUpJetPackControlHandler(PlayerController playerController, float duration, PowerUpSettings powerUpSettings)
    : base(playerController, duration: duration)
  {
    _powerUpSettings = powerUpSettings;

#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 0.2f, .5f, 1f);
#endif
  }

  public override void Dispose()
  {
#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
#endif
  }

  protected override bool DoUpdate()
  {
    // 1) when the player presses the Jump key, the jetpack will fire its engine and propel the player into the direction he his pointing towards
    // 2) when the Jump button is released, the player will still have some inertia before stopping in mid air
    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    var horizontalAxisState = GameManager.InputStateManager.GetAxisState("Horizontal");
    var verticalAxisState = GameManager.InputStateManager.GetAxisState("Vertical");

    if ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsPressed) != 0)
    {
      // we want to dash towards the direction the controller points to
      velocity.x = Mathf.Lerp(velocity.x, horizontalAxisState.Value * _powerUpSettings.JetpackSettings.JetpackSpeed, Time.deltaTime * _powerUpSettings.JetpackSettings.AirDamping);

      if (horizontalAxisState.Value == 0f)
      {
        velocity.y = Mathf.Lerp(velocity.y, 1f * _powerUpSettings.JetpackSettings.JetpackSpeed, Time.deltaTime * _powerUpSettings.JetpackSettings.AirDamping);
      }
      else
      {
        velocity.y = Mathf.Lerp(velocity.y, verticalAxisState.Value * _powerUpSettings.JetpackSettings.JetpackSpeed, Time.deltaTime * _powerUpSettings.JetpackSettings.AirDamping);
      }
    }
    else
    {
      if (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below == true)
      {
        velocity.y = 0f;

        velocity.y = Mathf.Max(
          GetGravityAdjustedVerticalVelocity(velocity, PlayerController.AdjustedGravity, true),
          PlayerController.JumpSettings.MaxDownwardSpeed);

        velocity.x = GetDefaultHorizontalVelocity(velocity);
      }
      else
      {
        if (_powerUpSettings.JetpackSettings.AutoFloatWithoutThrust)
        {
          if (velocity.x != 0f)
          {
            velocity.x = Mathf.Lerp(velocity.x, 0f, Time.deltaTime * _powerUpSettings.JetpackSettings.AirDamping);
          }

          if (velocity.y != 0f)
          {
            velocity.y = Mathf.Lerp(velocity.y, 0f, Time.deltaTime * _powerUpSettings.JetpackSettings.AirDamping);
          }

          if (velocity.x < 10f && velocity.x > -10f)
          {
            velocity.x = 0f;
          }

          if (velocity.y < 10f && velocity.y > -10f)
          {
            velocity.y = 0f;
          }
        }
        else
        {
          velocity.y = Mathf.Max(GetGravityAdjustedVerticalVelocity(velocity, _powerUpSettings.JetpackSettings.FloatGravity, true), PlayerController.JumpSettings.MaxDownwardSpeed);

          velocity.x = GetDefaultHorizontalVelocity(velocity);
        }
      }
    }

    if ((GameManager.InputStateManager.GetButtonState("Dash").ButtonPressState & ButtonPressState.IsPressed) != 0)
    {// fire
      var autoFireBulletsPerSecond = 10f; // TODO (Roman): hardcoded

      if (_lastBulletTime + (1 / autoFireBulletsPerSecond) <= Time.time)
      {
        var direction = new Vector2(horizontalAxisState.Value, verticalAxisState.Value).normalized;

        if (direction.x == 0f && direction.y == 0f)
        {
          direction.x = PlayerController.Sprite.transform.localScale.x > 0f ? 1f : -1f;
        }

        var bulletObject = ObjectPoolingManager.Instance.GetObject(GameManager.GameSettings.PooledObjects.BasicBullet.Prefab.name);

        var bullet = bulletObject.GetComponent<Bullet>();
        bullet.StartMove(PlayerController.transform.position, direction * 2000f); // TODO (Roman): hardcoded

        var rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bulletObject.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

        _lastBulletTime = Time.time;
      }
    }

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return true;
  }
}
