using TMPro;
using UnityEngine;

namespace DudeResqueSquad
{
    public class KeysHUD : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private TextMeshProUGUI _txtRegularKeys;
        [SerializeField] private TextMeshProUGUI _txtSpecialKeys;
        [SerializeField] private TextMeshProUGUI _txtSkeletonKeys;

        #endregion

        #region Private methods

        private void Start()
        {
            Debug.Log(_txtRegularKeys);

            GameManager.Instance.OnPlayerCollectKey += KeyCollected;
            GameManager.Instance.OnDoorOpened += DoorOpened;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnPlayerCollectKey -= KeyCollected;
            GameManager.Instance.OnDoorOpened -= DoorOpened;
        }

        private void DoorOpened(string doorUID, int keysAmount, PlayerData playerData)
        {
            _txtRegularKeys.text = $"x {playerData.RegularKeys}";
            _txtSpecialKeys.text = $"x {playerData.SpecialKeys}";
            _txtSkeletonKeys.text = $"x {playerData.SkeletonKeys}";
        }

        private void KeyCollected(Enums.KeyType keyType, PlayerData playerData)
        {
            if (keyType == Enums.KeyType.REGULAR)
                _txtRegularKeys.text = $"x {playerData.RegularKeys}";
            else if (keyType == Enums.KeyType.SPECIAL)
                _txtSpecialKeys.text = $"x {playerData.SpecialKeys}";
            if (keyType == Enums.KeyType.SKELETON)
                _txtSkeletonKeys.text = $"x {playerData.SkeletonKeys}";
        }

        #endregion
    }
}