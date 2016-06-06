using UnityEngine;

public class FlyingFollowerEnemyControlHandler : EnemyControlHandler<FlyingFollowerEnemyController>
{
  private PlayerController _playerController;

  private Vector3 _velocity = Vector3.zero;

  public FlyingFollowerEnemyControlHandler(FlyingFollowerEnemyController flyingFollowerEnemyController)
    : base(flyingFollowerEnemyController)
  {
    _playerController = GameManager.Instance.Player;
  }

  protected override bool DoUpdate()
  {
    var direction = _playerController.transform.position - _enemyController.transform.position;

    direction = direction.normalized * _enemyController.speed * Time.deltaTime;

    _velocity.x = Mathf.Lerp(_velocity.x, direction.x, _enemyController.smoothDampFactor * Time.deltaTime);
    _velocity.y = Mathf.Lerp(_velocity.y, direction.y, _enemyController.smoothDampFactor * Time.deltaTime);

    _enemyController.transform.Translate(_velocity);

    return true;
  }
}
