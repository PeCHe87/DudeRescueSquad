using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private string _uid = string.Empty;  // TODO: this will be replaced with DoorData
        [SerializeField] private int _keysRequired = 0;  // TODO: this will be replaced with DoorData
        [SerializeField] private Enums.KeyType _keyType = Enums.KeyType.NONE;  // TODO: this will be replaced with DoorData
        [SerializeField] private BoxCollider _blockCollider = null;
        [SerializeField] private GameObject _art = null;

        #region Private constants

        private const string _playerTag = "Player";

        #endregion

        #region Private properties

        private bool _isClosed = true;

        #endregion

        #region Private methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_playerTag))
            {
                Character character = other.GetComponent<Character>();

                PlayerData playerData = (character != null) ? character.Data : null;

                int keys = 0;

                if (_keyType == Enums.KeyType.REGULAR)
                    keys = (playerData != null) ? playerData.RegularKeys : 0;
                else if (_keyType == Enums.KeyType.SPECIAL)
                    keys = (playerData != null) ? playerData.SpecialKeys : 0;
                else if (_keyType == Enums.KeyType.SKELETON)
                    keys = (playerData != null) ? playerData.SkeletonKeys : 0;

                if (keys >= _keysRequired)
                    OpenDoor(playerData);
                else
                    ShowHowToOpen();
            }
        }

        private void ShowHowToOpen()
        {
            // TODO: invokes a method to show a message on UI floating popup with the amount and type of key required to open it

            Debug.Log($"Close door, you need <b>{_keysRequired}</b> keys of type <color=red><b>'{_keyType}'</b></color>");
        }

        private void OpenDoor(PlayerData playerData)
        {
            if (_keyType == Enums.KeyType.REGULAR)
                playerData.RegularKeys -= _keysRequired;
            else if (_keyType == Enums.KeyType.SPECIAL)
                playerData.SpecialKeys -= _keysRequired;
            else if (_keyType == Enums.KeyType.SKELETON)
                playerData.SkeletonKeys -= _keysRequired;

            _isClosed = false;

            _blockCollider.enabled = false;

            _art.SetActive(false);

            GameManager.Instance.OnDoorOpened?.Invoke(_uid, _keysRequired, playerData);
        }

        #endregion
    }
}