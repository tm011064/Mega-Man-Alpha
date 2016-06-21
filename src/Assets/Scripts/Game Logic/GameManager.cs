using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance = null;

  public PlayerController Player;

  [HideInInspector]
  public GameSettings GameSettings;

  [HideInInspector]
  public PowerUpManager PowerUpManager;

  [HideInInspector]
  public InputStateManager InputStateManager;

  [HideInInspector]
  public Easing Easing;

  private int _totalCoins = 0;

  private Checkpoint[] _orderedSceneCheckpoints;

  private int _currentCheckpointIndex = 0;

#if !FINAL
  private readonly FPSRenderer _fpsRenderer = new FPSRenderer();
#endif

  public void AddCoin()
  {
    _totalCoins++;
  }

  public void SpawnPlayerAtNextCheckpoint(bool doCycle)
  {
    if (_currentCheckpointIndex >= _orderedSceneCheckpoints.Length - 1)
    {
      if (doCycle)
      {
        _currentCheckpointIndex = 0;
      }
      else
      {
        _currentCheckpointIndex = _orderedSceneCheckpoints.Length - 1;
      }
    }
    else
    {
      _currentCheckpointIndex++;
    }

    var checkpoint = _orderedSceneCheckpoints[_currentCheckpointIndex].gameObject;

    Player.SpawnLocation = checkpoint.gameObject.transform.position;

    Player.Respawn();
  }

  public void SpawnPlayerAtCheckpoint(int checkpointIndex)
  {
    if (checkpointIndex < 0)
    {
      _currentCheckpointIndex = 0;
    }
    else if (checkpointIndex >= _orderedSceneCheckpoints.Length)
    {
      _currentCheckpointIndex = _orderedSceneCheckpoints.Length - 1;
    }
    else
    {
      _currentCheckpointIndex = checkpointIndex;
    }

    var checkpoint = _orderedSceneCheckpoints[_currentCheckpointIndex].gameObject;

    Player.SpawnLocation = checkpoint.gameObject.transform.position;

    Player.Respawn();
  }

  public void RefreshScene(Vector3 cameraPosition)
  {
    foreach (EnemySpawnManager enemySpawnManager in FindObjectsOfType<EnemySpawnManager>())
    {
      if (!enemySpawnManager.DestroySpawnedEnemiesWhenGettingDisabled)
      {
        enemySpawnManager.DeactivateSpawnedObjects();
      }
    }

    foreach (SpawnBucket spawnBucket in FindObjectsOfType<SpawnBucket>())
    {
      spawnBucket.Reload();
    }

    // TODO (Roman): have a flag for pooled objects that need to be deactivated

    var cameraController = Camera.main.GetComponent<CameraController>();

    cameraController.SetPosition(cameraPosition);

#if !FINAL
    _fpsRenderer.SceneStartTime = Time.time;
#endif
  }

  public void LoadScene()
  {
    GameObject checkpoint;

    // TODO (Roman): don't hardcode tags
    switch (SceneManager.GetActiveScene().name)
    {
      case "Platforms And Enemies":

        _orderedSceneCheckpoints = GameObject.FindObjectsOfType<Checkpoint>();

        Array.Sort(_orderedSceneCheckpoints, (a, b) => b.Index.CompareTo(a.Index));

        _currentCheckpointIndex = 0;

        checkpoint = _orderedSceneCheckpoints[_currentCheckpointIndex].gameObject;

        break;

      default:

        _orderedSceneCheckpoints = GameObject.FindObjectsOfType<Checkpoint>();

        Array.Sort(_orderedSceneCheckpoints, (a, b) => a.Index.CompareTo(b.Index));

        _currentCheckpointIndex = 0;

        checkpoint = _orderedSceneCheckpoints[_currentCheckpointIndex].gameObject;

        break;
    }

    // TODO (Roman): all those registrations should be optional
    var objectPoolingManager = ObjectPoolingManager.Instance;

    objectPoolingManager.DeactivateAndClearAll();

    objectPoolingManager.RegisterPool(
      GameSettings.PooledObjects.BasicPowerUpPrefab.Prefab,
      GameSettings.PooledObjects.BasicPowerUpPrefab.InitialSize,
      int.MaxValue);

    objectPoolingManager.RegisterPool(
      GameSettings.PooledObjects.BasicBullet.Prefab,
      GameSettings.PooledObjects.BasicBullet.InitialSize,
      int.MaxValue);

    objectPoolingManager.RegisterPool(
      GameSettings.PooledObjects.DefaultEnemyDeathParticlePrefab.Prefab,
      GameSettings.PooledObjects.DefaultEnemyDeathParticlePrefab.InitialSize,
      int.MaxValue);

    objectPoolingManager.RegisterPool(
      GameSettings.PooledObjects.DefaultPlayerDeathParticlePrefab.Prefab,
      GameSettings.PooledObjects.DefaultPlayerDeathParticlePrefab.InitialSize,
      int.MaxValue);

    var monoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();

    var gameObjectTypes = new Dictionary<string, ObjectPoolRegistrationInfo>();

    for (var i = 0; i < monoBehaviours.Length; i++)
    {
      var objectPoolBehaviour = monoBehaviours[i] as IObjectPoolBehaviour;

      if (objectPoolBehaviour != null)
      {
        foreach (var objectPoolRegistrationInfo in objectPoolBehaviour.GetObjectPoolRegistrationInfos())
        {
          if (gameObjectTypes.ContainsKey(objectPoolRegistrationInfo.GameObject.name))
          {
            if (gameObjectTypes[objectPoolRegistrationInfo.GameObject.name].TotalInstances < objectPoolRegistrationInfo.TotalInstances)
            {
              gameObjectTypes[objectPoolRegistrationInfo.GameObject.name] = objectPoolRegistrationInfo.Clone();
            }
          }
          else
          {
            gameObjectTypes[objectPoolRegistrationInfo.GameObject.name] = objectPoolRegistrationInfo.Clone();
          }
        }
      }
    }

    Logger.Info("Registering " + gameObjectTypes.Count + " objects at object pool.");

    foreach (ObjectPoolRegistrationInfo objectPoolRegistrationInfo in gameObjectTypes.Values)
    {
      objectPoolingManager.RegisterPool(
        objectPoolRegistrationInfo.GameObject,
        objectPoolRegistrationInfo.TotalInstances,
        int.MaxValue);
    }

    var playerController = Instantiate(
      GameManager.Instance.Player,
      checkpoint.transform.position,
      Quaternion.identity) as PlayerController;

    playerController.SpawnLocation = checkpoint.transform.position;

    Player = playerController;

#if !FINAL
    _fpsRenderer.SceneStartTime = Time.time;
#endif
  }

  void Awake()
  {
    Logger.Info("Awaking Game Manager at " + DateTime.Now.ToString());

    if (Instance == null)
    {
      Logger.Info("Setting Game Manager instance.");

      Instance = this;
    }
    else if (Instance != this)
    {
      Logger.Info("Destroying Game Manager instance.");

      Destroy(gameObject);
    }

    PowerUpManager = new PowerUpManager(this);

    InputStateManager = new InputStateManager();

    InputStateManager.InitializeButtons("Jump", "Dash", "Fall", "SwitchPowerUp", "Attack", "Aim");
    InputStateManager.InitializeAxes("Horizontal", "Vertical");

    Easing = new Easing();

    DontDestroyOnLoad(gameObject);
  }

  private void EvaluateCheatKeys()
  {
    if (Input.GetKeyUp("n"))
    {
      Debug.Log("Key Command: Go to next checkpoint");

      _currentCheckpointIndex--;

      if (_currentCheckpointIndex < 0)
      {
        _currentCheckpointIndex = _orderedSceneCheckpoints.Length - 1;
      }

      var checkpoint = _orderedSceneCheckpoints[_currentCheckpointIndex].gameObject;

      Player.SpawnLocation = checkpoint.gameObject.transform.position;

      Player.Respawn();
    }
    if (Input.GetKeyUp("p"))
    {
      Debug.Log("Key Command: Go to previous checkpoint");

      _currentCheckpointIndex++;

      if (_currentCheckpointIndex >= _orderedSceneCheckpoints.Length)
      {
        _currentCheckpointIndex = 0;
      }

      var checkpoint = _orderedSceneCheckpoints[_currentCheckpointIndex].gameObject;

      Player.SpawnLocation = checkpoint.gameObject.transform.position;

      Player.Respawn();
    }
    if (Input.GetKeyUp("z"))
    {
      Debug.Log("Key Command: add all powerups");

      GameManager.Instance.PowerUpManager.ApplyPowerUpItem(PowerUpType.Floater);

      GameManager.Instance.PowerUpManager.ApplyPowerUpItem(PowerUpType.DoubleJump);

      GameManager.Instance.PowerUpManager.ApplyPowerUpItem(PowerUpType.JetPack);

      GameManager.Instance.PowerUpManager.ApplyPowerUpItem(PowerUpType.Gun);
    }
  }

  void Update()
  {
    InputStateManager.Update();

    // TODO (Roman): this must not make it into release
    EvaluateCheatKeys();

    // TODO (Roman): this must not make it into release
    if (Input.GetKey("escape"))
    {
      Logger.Info("quit");

      Application.Quit();
    }

#if !FINAL
    _fpsRenderer.UpdateFPS();
#endif
  }

  void OnDestroy()
  {
    Logger.Destroy();
  }

#if !FINAL
  void OnGUI()
  {
    _fpsRenderer.RenderFPS();
  }
#endif
}
