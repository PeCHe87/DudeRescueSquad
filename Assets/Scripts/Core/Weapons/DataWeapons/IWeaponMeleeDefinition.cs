namespace DudeRescueSquad.Core.Weapons
{
    public interface IWeaponMeleeDefinition
    {
        float AttackRange { get; set; }
        float AttackDuration { get; set; }
        float AttackDamage { get; set; }
    }
}