using UnityEngine;

namespace DudeResqueSquad
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data")]
    public class PlayerData : ScriptableObject
    {
        public string UID;
        public ItemData CurrentItem;
    }
}