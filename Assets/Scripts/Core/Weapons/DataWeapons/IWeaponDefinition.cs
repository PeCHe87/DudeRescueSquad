using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    public interface IWeaponDefinition
    {
        WeaponType Type { get; set; }
        string Id { get; set; }
        string DisplayName { get; }
        float RadiusDetection { get; set; }
        float AngleView { get; set; }
        bool IsLeftHand { get; set; }
        bool CanMoveWhileAttacking { get; }
        Sprite Icon { get; }
        bool CanPushBackOnHit { get; }
    }
}