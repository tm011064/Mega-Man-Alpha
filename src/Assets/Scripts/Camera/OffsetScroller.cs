using UnityEngine;

public class OffsetScroller : MonoBehaviour // TODO (Roman): does this work?
{
  public float SpeedFactor = 2000f;

  private Vector2 _savedOffset;

  private Renderer _renderer;

  private Transform _transform;

  private Vector3 _oldPos;

  private Vector2 _lastOffset;

  private float _horizontalSmoothDampVelocity;

  private float _verticalSmoothDampVelocity;

  void Awake()
  {
    _renderer = GetComponent<Renderer>();

    _transform = Camera.main.transform;
  }

  void Start()
  {
    _savedOffset = _renderer.sharedMaterial.GetTextureOffset("_MainTex");

    _oldPos = _transform.position;

    _lastOffset = _savedOffset;
  }

  void LateUpdate()
  {
    var delta = _transform.position - _oldPos;

    var y = Mathf.Repeat(delta.y / SpeedFactor, 1);
    var x = Mathf.Repeat(delta.x / SpeedFactor, 1);

    _lastOffset = _lastOffset + new Vector2(x, y);

    _renderer.sharedMaterial.SetTextureOffset(
      "_MainTex",
      new Vector2(Mathf.Repeat(_lastOffset.x, 1), Mathf.Repeat(_lastOffset.y, 1)));

    _oldPos = _transform.position;
  }

  void OnDisable()
  {
    _renderer.sharedMaterial.SetTextureOffset("_MainTex", _savedOffset);
  }
}
