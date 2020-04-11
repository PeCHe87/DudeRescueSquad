using UnityEngine;

namespace DudeResqueSquad
{
    [CreateAssetMenu(fileName = "KeyData", menuName = "Data/Key Data")]
    public class ItemKeyData : ItemData
    {
        public Enums.KeyType KeyType;
    }
}