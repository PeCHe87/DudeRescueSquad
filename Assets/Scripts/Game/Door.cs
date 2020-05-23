using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private DoorData _data = null;
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

                if (_data.KeyType == Enums.KeyType.REGULAR)
                    keys = (playerData != null) ? playerData.RegularKeys : 0;
                else if (_data.KeyType == Enums.KeyType.SPECIAL)
                    keys = (playerData != null) ? playerData.SpecialKeys : 0;
                else if (_data.KeyType == Enums.KeyType.SKELETON)
                    keys = (playerData != null) ? playerData.SkeletonKeys : 0;

                if (keys >= _data.KeysRequired)
                    OpenDoor(playerData);
                else
                    ShowHowToOpen();
            }
        }

        private void ShowHowToOpen()
        {
            // TODO: invokes a method to show a message on UI floating popup with the amount and type of key required to open it

            Debug.Log($"Closed door, you need <b>{_data.KeysRequired}</b> keys of type <color=red><b>'{_data.KeyType}'</b></color>");
        }

        private void OpenDoor(PlayerData playerData)
        {
            if (_data.KeyType == Enums.KeyType.REGULAR)
                playerData.RegularKeys -= _data.KeysRequired;
            else if (_data.KeyType == Enums.KeyType.SPECIAL)
                playerData.SpecialKeys -= _data.KeysRequired;
            else if (_data.KeyType == Enums.KeyType.SKELETON)
                playerData.SkeletonKeys -= _data.KeysRequired;

            _isClosed = false;

            _blockCollider.enabled = false;

            _art.SetActive(false);

            // TODO: animate door

            GameManager.Instance.OnDoorOpened?.Invoke(_data.UID, _data.KeysRequired, playerData);
        }

        #endregion
    }
}