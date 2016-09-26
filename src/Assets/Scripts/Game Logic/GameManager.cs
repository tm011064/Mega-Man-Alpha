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
  public InputStateManager InputStateManager;

  [HideInInspector]
  public Easing Easing;

  private Checkpoint[] _orderedSceneCheckpoints;

  private int _currentCheckpointIndex = 0;

#if !FINAL
  private readonly FPSRenderer _fpsRenderer = new FPSRenderer();
#endif

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
    ObjectPoolingManager.Instance.DeactivateAll();

    ResetCameraPosition(cameraPosition);
    
    foreach (ISceneResetable sceneResetable in FindComponents<ISceneResetable>())
    {
      sceneResetable.OnSceneReset();
    }

#if !FINAL
    _fpsRenderer.SceneStartTime = Time.time;
#endif
  }

  private void ResetCameraPosition(Vector3 position)
  {
    var cameraController = Camera.main.GetComponent<CameraController>();

    cameraController.MoveCameraToTargetPosition(position);
  }

  public void LoadScene()
  {
    GameObject checkpoint;

    switch (SceneManager.GetActiveScene().name)
    {
      default:

        _orderedSceneCheckpoints = GameObject.FindObjectsOfType<Checkpoint>();

        Array.Sort(_orderedSceneCheckpoints, (a, b) => a.Index.CompareTo(b.Index));

        _currentCheckpointIndex = 0;

        checkpoint = _orderedSceneCheckpoints[_currentCheckpointIndex].gameObject;

        break;
    }

    ResetPooledObjects();

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

  private IEnumerable<T> FindComponents<T>()
    where T : class
  {
    var monoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();

    for (var i = 0; i < monoBehaviours.Length; i++)
    {
      var component = monoBehaviours[i] as T;

      if (component != null)
      {
        yield return component;
      }
    }
  }

  private void ResetPooledObjects()
  {
    var objectPoolingManager = ObjectPoolingManager.Instance;

    objectPoolingManager.DeactivateAndClearAll();

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

    InputStateManager = new InputStateManager();

    InputStateManager.InitializeButtons("Jump", "Dash", "Fall", "Attack");
    InputStateManager.InitializeAxes("Horizontal", "Vertical");

    Easing = new Easing();

    DontDestroyOnLoad(gameObject);
  }

  void Update()
  {
    InputStateManager.Update();

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
