#if UNITY_EDITOR

using UnityEngine;

public partial class FullScreenScroller : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments)
  {
    SetPosition(arguments);

    MustBeOnLadderToEnter = arguments.GetBool("Enter On Ladder");
  }

  private void SetPosition(InstantiationArguments arguments)
  {
    BoundaryPadding = new Padding
    {
      Left = arguments.GetBool("Open Left") ? arguments.GetInt("Extension Left") : 0,
      Right = arguments.GetBool("Open Right") ? arguments.GetInt("Extension Right") : 0,
      Top = arguments.GetBool("Open Top") ? arguments.GetInt("Extension Top") : 0,
      Bottom = arguments.GetBool("Open Bottom") ? arguments.GetInt("Extension Bottom") : 0
    };

    var xPos = arguments.Bounds.center.x + (BoundaryPadding.Right - BoundaryPadding.Left) / 2;

    var yPos = arguments.Bounds.center.y + (BoundaryPadding.Top - BoundaryPadding.Bottom) / 2;

    var width = arguments.Bounds.size.x + BoundaryPadding.Left + BoundaryPadding.Right;

    var height = arguments.Bounds.size.y + BoundaryPadding.Bottom + BoundaryPadding.Top;

    var verticalLockPositionAdjustment = (BoundaryPadding.Bottom - BoundaryPadding.Top) / 2;

    transform.position = new Vector2(xPos, yPos);

    EnableDefaultVerticalLockPosition = true;
    DefaultVerticalLockPosition = transform.position.y + verticalLockPositionAdjustment;

    var boxCollider = GetComponent<BoxCollider2D>();

    if (boxCollider == null)
    {
      throw new MissingComponentException("BoxCollider2D expected");
    }

    boxCollider.size = new Vector2(width, height);
  }

  public bool Contains(Vector2 point)
  {
    var boxCollider = GetComponent<BoxCollider2D>();

    return boxCollider.bounds.Contains(point);
  }
}

#endif