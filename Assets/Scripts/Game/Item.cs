using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class Item : MonoBehaviour
    {
        #region Events

        public static Action<CustomEventArgs.CollectItemEventArgs> OnCollected;
        public static Action<ItemData, string> OnCollect;

        #endregion

        #region Inspector properties

        [SerializeField] private ItemData _data = null;
        [SerializeField] private GameObject _graphic = null;
        [SerializeField] private float _destroyTime = 2;

        #endregion

        #region Private constants

        private const string _playerTag = "Player";

        #endregion

        #region Private methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_playerTag))
            {
                Character character = other.GetComponent<Character>();

                PlayerData playerData = (character != null) ? character.Data : null;

                string playerId = (playerData != null) ? playerData.UID : string.Empty;

                Collect(playerId);
            }
        }

        protected virtual void Collect(string playerId)
        {
            // TODO: check if it should replace the current item that player has in this place

            // Destroy item
            DestroyIt();

            OnCollect?.Invoke(_data, playerId);
            OnCollected?.Invoke(new CustomEventArgs.CollectItemEventArgs(_data, playerId));
        }

        private void DestroyIt()
        {
            // TODO: show some VFX

            // TODO: play a SFX

            // Hide graphic
            _graphic.SetActive(false);

            // Destroy the object with delay
            Destroy(gameObject, _destroyTime);
        }

        #endregion
    }
}