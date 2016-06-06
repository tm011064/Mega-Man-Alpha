using UnityEngine;

public class BezierCurve : MonoBehaviour
{
  public Vector3[] Points;

  public Vector3 GetPoint(float t)
  {
    return transform.TransformPoint(Bezier.GetPoint(Points[0], Points[1], Points[2], Points[3], t));
  }

  public Vector3 GetVelocity(float t)
  {
    return transform.TransformPoint(Bezier.GetFirstDerivative(Points[0], Points[1], Points[2], Points[3], t)) - transform.position;
  }

  public Vector3 GetDirection(float t)
  {
    return GetVelocity(t).normalized;
  }

  public void Reset()
  {
    Points = new Vector3[] 
    {
      new Vector3(1f, 0f, 0f),
      
      new Vector3(2f, 0f, 0f),
      
      new Vector3(3f, 0f, 0f),
      
      new Vector3(4f, 0f, 0f)
    };
  }
}
