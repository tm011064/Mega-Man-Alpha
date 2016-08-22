#if UNITY_EDITOR

using UnityEngine;

public partial class BoxColliderTriggerEnterBehaviour : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments)
  {
    var padding = new Padding
    {
      Left = arguments.GetBool("Open Left") ? arguments.GetInt("Extension Left") : 0,
      Right = arguments.GetBool("Open Right") ? arguments.GetInt("Extension Right") : 0,
      Top = arguments.GetBool("Open Top") ? arguments.GetInt("Extension Top") : 0,
      Bottom = arguments.GetBool("Open Bottom") ? arguments.GetInt("Extension Bottom") : 0
    };

    var width = arguments.Bounds.size.x + padding.Left + padding.Right;

    var height = arguments.Bounds.size.y + padding.Bottom + padding.Top;

    var boxCollider = this.GetComponentOrThrow<BoxCollider2D>();

    boxCollider.size = new Vector2(width, height);
  }
}

#endif
