namespace Assets.Editor.Tiled
{
  public class Edge
  {
    public static Edge NullEdge = new Edge();

    public Vertex From { get; set; }

    public Vertex To { get; set; }

    public bool IsColliderEdge { get; set; }

    public override string ToString()
    {
      return "IsColliderEdge: " + IsColliderEdge;
    }
  }
}