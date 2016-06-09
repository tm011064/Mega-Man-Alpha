using UnityEngine;

public class PowerUpGunControlHandler : DefaultPlayerControlHandler
{
  private PowerUpSettings _powerUpSettings;

  private float _slowMotionTotalTime;

  private float _lastAimStartTime;

  private float _nextAvailableAimStartTime;

  private float _lastBulletTime;

  private bool _isAiming;

  private float _autoAimAngleStep;

  public PowerUpGunControlHandler(PlayerController playerController, float duration, PowerUpSettings powerUpSettings)
    : base(playerController, duration)
  {
    _powerUpSettings = powerUpSettings;

    _slowMotionTotalTime = _powerUpSettings.LaserAimGunSettings.SlowMotionFactorMultplierCurve.keys[_powerUpSettings.LaserAimGunSettings.SlowMotionFactorMultplierCurve.keys.Length - 1].time;

    if (_powerUpSettings.LaserAimGunSettings.DoAutoAim)
    {
      Logger.Assert(_powerUpSettings.LaserAimGunSettings.TotalAutoAimSearchRaysPerSide != 0, "TotalAutoAimSearchRaysPerSide must not be 0.");

      _autoAimAngleStep = _powerUpSettings.LaserAimGunSettings.AutoAimSearchAngle
        * Mathf.Deg2Rad
        * .5f
        / (float)_powerUpSettings.LaserAimGunSettings.TotalAutoAimSearchRaysPerSide;
    }

#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.4f, .3f, 1f);
#endif
  }

  public override void Dispose()
  {
    // reset stuff
    HorizontalAxisOverride = null;

    VerticalAxisOverride = null;

    Time.timeScale = 1f;

    _isAiming = false;

#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
#endif
  }

  [System.Diagnostics.Conditional("DEBUG")]
  private void DrawRay(Vector3 start, Vector3 dir, Color color)
  {
    Debug.DrawRay(start, dir, color);
  }

  private void EvaluateShot(Vector2 aimVector)
  {
    if ((GameManager.InputStateManager.GetButtonState("Dash").ButtonPressState & ButtonPressState.IsDown) != 0)
    {// fire
      if (_lastBulletTime + (1 / _powerUpSettings.LaserAimGunSettings.BulletsPerSecond)
        * (_powerUpSettings.LaserAimGunSettings.AllowSlowMotionRealTimeBulletsPerSecond ? Time.timeScale : 1f)
        <= Time.time)
      {
        var direction = aimVector.normalized;

        if (direction.x == 0f && direction.y == 0f)
        {
          direction.x = PlayerController.Sprite.transform.localScale.x > 0f ? 1f : -1f;
        }

        var bulletObject = ObjectPoolingManager.Instance.GetObject(GameManager.GameSettings.PooledObjects.BasicBullet.Prefab.name, PlayerController.transform.position);

        var bullet = bulletObject.GetComponent<Bullet>();

        bullet.StartMove(PlayerController.transform.position, direction * _powerUpSettings.LaserAimGunSettings.BulletSpeed); // TODO (Roman): hardcoded

        var rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bulletObject.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

        _lastBulletTime = Time.time;
      }
    }
  }

  private void PerformAimAction()
  {
    AxisState horizontalAxisState = GameManager.InputStateManager.GetAxisState("Horizontal");
    AxisState verticalAxisState = GameManager.InputStateManager.GetAxisState("Vertical");

    if ((GameManager.InputStateManager.GetButtonState("Aim").ButtonPressState & ButtonPressState.IsDown) != 0)
    {
      if (Time.unscaledTime >= _nextAvailableAimStartTime)
      {
        _lastAimStartTime = Time.unscaledTime;

        if (!PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
        {// we are in air, so we want to keep momentum
          HorizontalAxisOverride = horizontalAxisState.Clone();
          VerticalAxisOverride = verticalAxisState.Clone();
        }

        _isAiming = true;
      }
    }
    else if ((GameManager.InputStateManager.GetButtonState("Aim").ButtonPressState & ButtonPressState.IsPressed) != 0)
    {
      if (_isAiming)
      {
        if (_lastAimStartTime + _slowMotionTotalTime < Time.unscaledTime)
        {
          _nextAvailableAimStartTime = Time.unscaledTime + _powerUpSettings.LaserAimGunSettings.IntervalBetweenAiming;

          _isAiming = false;
        }
      }
    }
    else if ((GameManager.InputStateManager.GetButtonState("Aim").ButtonPressState & ButtonPressState.IsUp) != 0)
    {
      if (_isAiming)
      {
        _nextAvailableAimStartTime = Time.unscaledTime + _powerUpSettings.LaserAimGunSettings.IntervalBetweenAiming;

        _isAiming = false;
      }
    }
    else
    {
      _isAiming = false;
    }

    if (_isAiming)
    {
      Time.timeScale = _powerUpSettings.LaserAimGunSettings.SlowMotionFactorMultplierCurve.Evaluate(
        Time.unscaledTime - _lastAimStartTime);

      PlayerController.LaserGunAimContainer.Activate(); // make sure it is active

      var aimVector = new Vector3(horizontalAxisState.Value, verticalAxisState.Value);

      if (_powerUpSettings.LaserAimGunSettings.DoAutoAim)
      {
        // first we shoot a straight ray towards the axis position
        var straightRayHit = Physics2D.Raycast(
          PlayerController.transform.position,
          aimVector.normalized,
          _powerUpSettings.LaserAimGunSettings.ScanRayLength,
          verticalAxisState.Value > 0f
            ? _powerUpSettings.LaserAimGunSettings.ScanRayDirectionUpCollisionLayers
            : _powerUpSettings.LaserAimGunSettings.ScanRayDirectionDownCollisionLayers);

        if (straightRayHit && straightRayHit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {// we have a direct hit on an enemy, set the aim vector
          aimVector = straightRayHit.collider.gameObject.transform.position - PlayerController.transform.position;
        }
        else
        {// no direct enemy hit, now go towards the outer search angle limits and test whether we hit an enemy
          var hasHit = false;

          var straightVectorTheta = Mathf.Atan2(aimVector.y, aimVector.x);

          for (var i = 1; i <= _powerUpSettings.LaserAimGunSettings.TotalAutoAimSearchRaysPerSide; i++)
          {
            // test right first
            var theta = straightVectorTheta + (float)i * _autoAimAngleStep;

            var searchVector = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));

            DrawRay(PlayerController.transform.position, searchVector.normalized * _powerUpSettings.LaserAimGunSettings.ScanRayLength, Color.yellow);

            var raycastHit2D = Physics2D.Raycast(
              PlayerController.transform.position,
              searchVector.normalized,
              _powerUpSettings.LaserAimGunSettings.ScanRayLength,
              verticalAxisState.Value > 0f
                ? _powerUpSettings.LaserAimGunSettings.ScanRayDirectionUpCollisionLayers
                : _powerUpSettings.LaserAimGunSettings.ScanRayDirectionDownCollisionLayers);

            if (raycastHit2D && raycastHit2D.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
              aimVector = raycastHit2D.collider.gameObject.transform.position - PlayerController.transform.position;

              hasHit = true;

              break;
            }

            // then left
            theta = straightVectorTheta - (float)i * _autoAimAngleStep;

            searchVector = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));

            DrawRay(PlayerController.transform.position, searchVector.normalized * _powerUpSettings.LaserAimGunSettings.ScanRayLength, Color.yellow);

            raycastHit2D = Physics2D.Raycast(
              PlayerController.transform.position,
              searchVector.normalized,
              _powerUpSettings.LaserAimGunSettings.ScanRayLength,
              verticalAxisState.Value > 0f
                ? _powerUpSettings.LaserAimGunSettings.ScanRayDirectionUpCollisionLayers
                : _powerUpSettings.LaserAimGunSettings.ScanRayDirectionDownCollisionLayers);

            if (raycastHit2D && raycastHit2D.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
              aimVector = raycastHit2D.collider.gameObject.transform.position - PlayerController.transform.position;

              hasHit = true;

              break;
            }
          }

          if (!hasHit)
          {// no enemy hit
            if (straightRayHit)
            {// if we initially hit something other than an enemy, use the hitpoint
              aimVector = straightRayHit.point.ToVector3() - PlayerController.transform.position;
            }
            else
            {// otherwise, just aim the full length
              aimVector = aimVector.normalized * _powerUpSettings.LaserAimGunSettings.ScanRayLength;
            }
          }
        }
      }
      else
      {
        var raycastHit2D = Physics2D.Raycast(
          PlayerController.transform.position,
          aimVector.normalized,
          _powerUpSettings.LaserAimGunSettings.ScanRayLength,
          verticalAxisState.Value > 0f
            ? _powerUpSettings.LaserAimGunSettings.ScanRayDirectionUpCollisionLayers
            : _powerUpSettings.LaserAimGunSettings.ScanRayDirectionDownCollisionLayers);

        if (raycastHit2D)
        {
          aimVector = raycastHit2D.point.ToVector3() - PlayerController.transform.position;
        }
        else
        {
          aimVector = aimVector.normalized * _powerUpSettings.LaserAimGunSettings.ScanRayLength;
        }
      }

      PlayerController.LaserGunAimContainer.UpdateAimAngle(aimVector);

      EvaluateShot(aimVector);
    }
    else
    {
      // reset stuff
      HorizontalAxisOverride = null;

      VerticalAxisOverride = null;

      Time.timeScale = 1f;

      _isAiming = false;

      PlayerController.LaserGunAimContainer.Deactivate(); // make sure it is inactive

      var aimVector = new Vector2(horizontalAxisState.Value, verticalAxisState.Value);

      if (_powerUpSettings.LaserAimGunSettings.AimHelpAngle > 0f)
      {
        var theta = Mathf.Atan2(aimVector.y, aimVector.x);

        if (theta < 0f)
        {
          theta = 2 * Mathf.PI + theta;
        }

        var aimHelpAngle = _powerUpSettings.LaserAimGunSettings.AimHelpAngle * Mathf.Deg2Rad;

        theta += aimHelpAngle * .5f;

        theta = ((float)((int)(theta / aimHelpAngle))) * aimHelpAngle;

        aimVector = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
      }

      EvaluateShot(aimVector);
    }
  }

  public override void OnAfterStackPeekUpdate()
  {
    PerformAimAction();
  }

  protected override bool DoUpdate()
  {
    PerformAimAction();

    return base.DoUpdate();
  }
}