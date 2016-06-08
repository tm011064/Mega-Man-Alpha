using UnityEngine;

public class GameLoader : MonoBehaviour
{
  public GameObject GameManagerPrefab;

  public GameSettings GameSettings;

  void Awake()
  {
    Logger.Initialize(GameSettings.LogSettings);

    if (GameManager.Instance == null)
    {
      Instantiate(GameManagerPrefab);
    }

    GameManager.Instance.GameSettings = GameSettings;

    // TODO (Roman): sound placeholder
    //if (SoundManager.instance == null)
    //  Instantiate(soundManager);

    // it is important to call that here as it instanciates the player controller which needs to have the game settings set.
    GameManager.Instance.LoadScene();
  }
}