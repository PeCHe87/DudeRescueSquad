namespace DudeResqueSquad.Weapons
{
    /**
     * This class is an abstract representation of a Weapon.
     * It controls how to use it, no matter if it is an assault or a melee weapon
    */
    public interface IWeapon
    {
        int Magazine();

        int CurrentBullets();
        
        bool IsAutoFire();

        bool CanFire();
        
        void Fire();

        void StopFire();

        void Reload();
    }
}