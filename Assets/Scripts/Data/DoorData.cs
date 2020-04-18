using UnityEngine;

namespace DudeResqueSquad
{
    [CreateAssetMenu(fileName = "DoorData", menuName = "Data/Door Data")]
    public class DoorData : ScriptableObject
    {
        public string UID;
        public int KeysRequired;
        public Enums.KeyType KeyType;
        public string DisplayName;
    }
}