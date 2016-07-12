using UnityEngine;

public partial class SpawnBucket : BaseMonoBehaviour, ISceneResetable
{
  [SerializeField]
  [InspectorReadOnlyAttribute]
  [Tooltip("Don't edit this, use the 'Register Child Objects' button instead")]
  private SpawnBucketItemBehaviour[] _children = new SpawnBucketItemBehaviour[0];

  public float VisibiltyCheckInterval = .1f;

  protected override void OnGotVisible()
  {
    for (var i = 0; i < _children.Length; i++)
    {
      _children[i].gameObject.SetActive(true);
    }
  }

  protected override void OnGotHidden()
  {
    for (var i = 0; i < _children.Length; i++)
    {
      _children[i].gameObject.SetActive(false);
    }
  }

  protected override void OnEnable()
  {
    for (var i = 0; i < _children.Length; i++)
    {
      if (_children[i].gameObject.activeSelf)
      {
        _children[i].gameObject.SetActive(false);
      }
    }

    base.OnEnable();
  }

  public void OnSceneReset()
  {
    for (var i = 0; i < _children.Length; i++)
    {
      if (_children[i].gameObject.activeSelf)
      {
        _children[i].gameObject.SetActive(false);
      }
    }

    IsVisible = false;
  }

  void Start()
  {
    StartVisibilityChecks(VisibiltyCheckInterval, GetComponent<Collider2D>());
  }
}