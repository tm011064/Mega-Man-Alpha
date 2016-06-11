using UnityEngine;

public class DontGoThroughThings : MonoBehaviour
{
  [Tooltip("All layers that the scan rays can collide with. Should include platforms and player.")]
  public LayerMask ScanRayDirectionDownCollisionLayers = 0;

  [Tooltip("All layers that the scan rays can collide with. Should include platforms and player.")]
  public LayerMask ScanRayDirectionUpCollisionLayers = 0;

  public float SkinWidth = 0.1f;

  private float _minimumExtent;

  private float _squareMinimumExtent;

  private Vector3 _previousPosition;

  private Collider2D _collider;

  private bool _skipFirstFrame;

  void OnEnable()
  {
    _previousPosition = gameObject.transform.position;
  }

  void Awake()
  {
    _collider = GetComponent<Collider2D>();
  }

  void Start()
  {
    _minimumExtent = Mathf.Min(Mathf.Min(_collider.bounds.extents.x, _collider.bounds.extents.y), _collider.bounds.extents.z);

    _squareMinimumExtent = _minimumExtent * _minimumExtent;
  }

  void Update()
  {
    var movementThisStep = gameObject.transform.position - _previousPosition;

    if (movementThisStep.magnitude > _squareMinimumExtent)
    {
      var raycastHit = Physics2D.Raycast(
        _previousPosition,
        movementThisStep.normalized,
        movementThisStep.magnitude,
        movementThisStep.y > 0f
          ? ScanRayDirectionUpCollisionLayers
          : ScanRayDirectionDownCollisionLayers);

      if (raycastHit)
      {
        _collider.SendMessage("OnTriggerEnter2D", raycastHit.collider, SendMessageOptions.DontRequireReceiver);
      }
    }

    _previousPosition = gameObject.transform.position;
  }
}