using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class Item : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private ItemData _data = null;
        [SerializeField] private GameObject _graphic = null;
        [SerializeField] private float _destroyTime = 2;

        [Header("Initial data")]
        [SerializeField] private float _initialDurability = 0;
        [SerializeField] private int _initialBullets = 0;

        #endregion

        #region Private constants

        private const string _playerTag = "Player";

        #endregion

        #region Private methods

        private void Start()
        {
            if (_data == null)
                return;

            if (_data is ItemWeaponData)
            {
                ItemWeaponData weapon = (ItemWeaponData)_data;

                weapon.CurrentDurability = _initialDurability;

                // Load current magazine bullets
                weapon.CurrentBulletsMagazine = (_initialBullets <= weapon.BulletsMagazine) ? _initialBullets : weapon.BulletsMagazine;
                
                // Load total amount of remaining bullets
                weapon.CurrentBulletsAmount = Mathf.Clamp(_initialBullets - weapon.CurrentBulletsMagazine, 0, _initialBullets);

                // Clean data
                weapon.IsReloading = false;
                weapon.RemainingReloadTime = 0;
                weapon.BulletsToReload = 0;
                weapon.RemainingDurabilityRecoveryTime = 0;
                weapon.IsRecoveringDurability = false;
            }
        }

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

            GameEvents.OnCollectItem?.Invoke(this, new CustomEventArgs.CollectItemEventArgs(_data, playerId));
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