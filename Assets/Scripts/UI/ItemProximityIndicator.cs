using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DudeResqueSquad
{
    public class ItemProximityIndicator : MonoBehaviour
    {
        [SerializeField] private Canvas _uiproximity = null;

        private const string _playerTag = "Player";

        private List<string> _charactersNear = null;

        private void Awake()
        {
            _uiproximity.enabled = false;

            _charactersNear = new List<string>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_playerTag))
            {
                Character character = other.GetComponent<Character>();

                PlayerData playerData = (character != null) ? character.Data : null;

                string playerId = (playerData != null) ? playerData.UID : string.Empty;

                if (playerId.Equals(string.Empty))
                    return;

                if (_charactersNear.Contains(playerId))
                    return;

                _charactersNear.Add(playerId);

                // First character near show the UI
                if (_charactersNear.Count == 1)
                    _uiproximity.enabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(_playerTag))
            {
                Character character = other.GetComponent<Character>();

                PlayerData playerData = (character != null) ? character.Data : null;

                string playerId = (playerData != null) ? playerData.UID : string.Empty;

                if (playerId.Equals(string.Empty))
                    return;

                if (!_charactersNear.Contains(playerId))
                    return;

                _charactersNear.Remove(playerId);

                // If there isn't any character near hide the UI
                if (_charactersNear.Count == 0)
                    _uiproximity.enabled = false;
            }
        }
    }
}