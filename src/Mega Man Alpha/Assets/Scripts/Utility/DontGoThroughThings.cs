using UnityEngine;
public class DontGoThroughThings : MonoBehaviour
{
  [Tooltip("All layers that the scan rays can collide with. Should include platforms and player.")]
  public LayerMask ScanRayDirectionDownCollisionLayers = 0;

  [Tooltip("All layers that the scan rays can collide with. Should include platforms and player.")]
  public LayerMask ScanRayDirectionUpCollisionLayers = 0;

  public float SkinWidth = 0.1f; //probably doesn't need to be changed 

  private float MinimumExtent;

  private float PartialExtent;

  private float SquareMinimumExtent;

  private Vector3 PreviousPosition;

  private Collider2D Collider;

  private bool _skipFirstFrame;

  //initialize values 
  void OnEnable()
  {
    PreviousPosition = gameObject.transform.position;
  }

  void Awake()
  {
    Collider = GetComponent<Collider2D>();
  }

  void Start()
  {
    MinimumExtent = Mathf.Min(Mathf.Min(Collider.bounds.extents.x, Collider.bounds.extents.y), Collider.bounds.extents.z);

    PartialExtent = MinimumExtent * (1.0f - SkinWidth);

    SquareMinimumExtent = MinimumExtent * MinimumExtent;
  }

  void Update()
  {
    var movementThisStep = gameObject.transform.position - PreviousPosition;

    if (movementThisStep.magnitude > SquareMinimumExtent)
    {
      //check for obstructions we might have missed 
      var raycastHit = Physics2D.Raycast(
        PreviousPosition,
        movementThisStep.normalized,
        movementThisStep.magnitude,
        movementThisStep.y > 0f
          ? ScanRayDirectionUpCollisionLayers
          : ScanRayDirectionDownCollisionLayers);

      if (raycastHit)
      {
        Collider.SendMessage("OnTriggerEnter2D", raycastHit.collider, SendMessageOptions.DontRequireReceiver);
      }
    }

    PreviousPosition = gameObject.transform.position;
  }
}