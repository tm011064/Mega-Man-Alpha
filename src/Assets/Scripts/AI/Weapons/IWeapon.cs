public interface IWeapon
{
  WeaponControlHandler CreateControlHandler(PlayerController playerController);
}