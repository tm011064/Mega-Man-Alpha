using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public class Vertex
  {
    private Dictionary<Direction, Edge> _edges = new Dictionary<Direction, Edge>
    {
      { Direction.Left, Edge.NullEdge },
      { Direction.Up, Edge.NullEdge },
      { Direction.Right, Edge.NullEdge },
      { Direction.Down, Edge.NullEdge }
    };

    public Vertex(Vector2 point)
    {
      Point = point;
    }

    public bool IsColliderPoint { get; set; }

    public bool IsVisited { get; set; }

    public Dictionary<Direction, Edge> Edges { get { return _edges; } }

    public Vector2 Point { get; private set; }

    public bool AreAllEdgesColliders()
    {
      return _edges[Direction.Left].IsColliderEdge
        && _edges[Direction.Up].IsColliderEdge
        && _edges[Direction.Right].IsColliderEdge
        && _edges[Direction.Down].IsColliderEdge;
    }

    public bool HasNoColliderEdges()
    {
      return !_edges[Direction.Left].IsColliderEdge
        && !_edges[Direction.Up].IsColliderEdge
        && !_edges[Direction.Right].IsColliderEdge
        && !_edges[Direction.Down].IsColliderEdge;
    }

    public override string ToString()
    {
      return Point.ToString() + " " + string.Join(", ", Edges.Select(kvp => kvp.Key + " -> " + kvp.Value).ToArray());
    }
  }
}