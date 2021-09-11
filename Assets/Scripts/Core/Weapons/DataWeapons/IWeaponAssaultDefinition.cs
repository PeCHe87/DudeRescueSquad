namespace DudeRescueSquad.Core.Weapons
{
    public interface IWeaponAssaultDefinition
    {
        SimpleProjectileData ProjectileData { get; set; }
        int MaxAmmo { get; set; }
        int InitialAmmo { get; set; }
        int AmmoConsumptionPerShot { get; }
        float ReloadingTime { get; set; }
        bool IsAutoFire { get; set; }
        float FireRate { get; set; }
        int MinAmountBulletsPerShot { get; }
        int MaxAmountBulletsPerShot { get; }
    }
}