using UnityEngine;

namespace DudeResqueSquad
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string UID;
        public string DisplayName;
    }
}