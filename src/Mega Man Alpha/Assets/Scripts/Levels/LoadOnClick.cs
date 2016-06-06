using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadOnClick : MonoBehaviour
{
  private AsyncOperation Async;

  public GameObject LoadingImage;

  public Text LoadingText;

  public void LoadScene(int level)
  {
    LoadingImage.SetActive(true);

    StartCoroutine(LoadLevelWithBar(level));
  }

  IEnumerator LoadLevelWithBar(int level)
  {
    Async = Application.LoadLevelAsync(level);

    while (!Async.isDone)
    {
      LoadingText.text = "Loading " + Async.progress;

      yield return null;
    }
  }
}
