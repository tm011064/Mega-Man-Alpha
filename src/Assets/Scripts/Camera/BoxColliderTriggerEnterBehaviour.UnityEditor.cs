#if UNITY_EDITOR

using UnityEngine;

public partial class BoxColliderTriggerEnterBehaviour : IInstantiable<CameraModifierInstantiationArguments>, IInstantiable<InstantiationArguments>
{
  public void Instantiate(CameraModifierInstantiationArguments arguments)
  {
    DoInstantiate(arguments);
  }

  public void Instantiate(InstantiationArguments arguments)
  {
    DoInstantiate(arguments);
  }

  private void DoInstantiate<TInstantiationArguments>(TInstantiationArguments instantiationArguments)
    where TInstantiationArguments : InstantiationArguments
  {
    var padding = new Padding
    {
      Left = instantiationArguments.Properties.GetBool("Open Left") ? instantiationArguments.Properties.GetInt("Extension Left") : 0,
      Right = instantiationArguments.Properties.GetBool("Open Right") ? instantiationArguments.Properties.GetInt("Extension Right") : 0,
      Top = instantiationArguments.Properties.GetBool("Open Top") ? instantiationArguments.Properties.GetInt("Extension Top") : 0,
      Bottom = instantiationArguments.Properties.GetBool("Open Bottom") ? instantiationArguments.Properties.GetInt("Extension Bottom") : 0
    };

    var width = instantiationArguments.Bounds.size.x + padding.Left + padding.Right;

    var height = instantiationArguments.Bounds.size.y + padding.Bottom + padding.Top;

    var boxCollider = this.GetComponentOrThrow<BoxCollider2D>();

    boxCollider.size = new Vector2(width, height);
  }
}

#endif
