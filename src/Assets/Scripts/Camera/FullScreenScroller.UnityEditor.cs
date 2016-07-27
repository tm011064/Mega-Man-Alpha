#if UNITY_EDITOR

using UnityEngine;

public partial class FullScreenScroller : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments)
  {
    var xPos = arguments.Bounds.center.x;
    var yPos = arguments.Bounds.center.y;
    var width = arguments.Bounds.size.x;
    var height = arguments.Bounds.size.y;
    var verticalLockPositionAdjustment = 0;

    if (arguments.GetBool("Open Left"))
    {
      xPos -= arguments.GetInt("Extension Left") / 2;
      width += arguments.GetInt("Extension Left");
    }

    if (arguments.GetBool("Open Right"))
    {
      xPos += arguments.GetInt("Extension Right") / 2;
      width += arguments.GetInt("Extension Right");
    }

    if (arguments.GetBool("Open Top"))
    {
      yPos += arguments.GetInt("Extension Top") / 2;
      height += arguments.GetInt("Extension Top");

      verticalLockPositionAdjustment -= arguments.GetInt("Extension Top") / 2;
    }

    if (arguments.GetBool("Open Bottom"))
    {
      yPos -= arguments.GetInt("Extension Bottom") / 2;
      height += arguments.GetInt("Extension Bottom");

      verticalLockPositionAdjustment += arguments.GetInt("Extension Bottom") / 2;
    }

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
}

#endif