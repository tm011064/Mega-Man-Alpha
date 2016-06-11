using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadOnClick : MonoBehaviour
{
  private AsyncOperation _async;

  public GameObject LoadingImage;

  public Text LoadingText;

  public void LoadScene(int level)
  {
    LoadingImage.SetActive(true);

    StartCoroutine(LoadLevelWithBar(level));
  }

  IEnumerator LoadLevelWithBar(int level)
  {
    _async = SceneManager.LoadSceneAsync(level);

    while (!_async.isDone)
    {
      LoadingText.text = "Loading " + _async.progress;

      yield return null;
    }
  }
}
