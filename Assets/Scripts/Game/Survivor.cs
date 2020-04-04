using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class Survivor : MonoBehaviour
    {
        #region Events

        public static Action<SurvivorData, string> OnCollect;

        #endregion

        #region Inspector properties

        [SerializeField] private SurvivorData _data = null;
        [SerializeField] private GameObject _graphic = null;
        [SerializeField] private float _destroyTime = 2;
        [SerializeField] private Animator _animator = null;

        #endregion

        #region Private constants

        private const string _playerTag = "Player";
        private string _wavingKey = "Waving";

        #endregion

        #region Private properties

        private ItemProximityIndicator _proximityIndicator = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            _proximityIndicator = GetComponentInChildren<ItemProximityIndicator>();

            if (_proximityIndicator != null)
            {
                _proximityIndicator.OnStartDetection += SomeoneNear;
                _proximityIndicator.OnStopDetection += AnyoneNear;
            }
        }

        private void OnDestroy()
        {
            if (_proximityIndicator != null)
            {
                _proximityIndicator.OnStartDetection -= SomeoneNear;
                _proximityIndicator.OnStopDetection -= AnyoneNear;
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
            // Destroy item
            DestroyIt();

            OnCollect?.Invoke(_data, playerId);
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

        private void AnyoneNear()
        {
            _animator.SetBool(_wavingKey, false);
        }

        private void SomeoneNear()
        {
            _animator.SetBool(_wavingKey, true);
        }

        #endregion
    }
}