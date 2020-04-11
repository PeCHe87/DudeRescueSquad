using UnityEngine;

namespace DudeResqueSquad
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data")]
    public class PlayerData : ScriptableObject
    {
        public string UID;
        public ItemWeaponData CurrentWeaponEquipped;
        public int RegularKeys;
        public int SpecialKeys;
        public int SkeletonKeys;

        public void Clean()
        {
            CurrentWeaponEquipped = null;
            RegularKeys = 0;
            SpecialKeys = 0;
            SkeletonKeys = 0;
        }
    }
}