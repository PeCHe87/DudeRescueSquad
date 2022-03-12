using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.LevelManagement.View;
using System.Collections;
using UnityEngine;

namespace DudeRescueSquad.Core.LevelManagement
{
    public class LevelDoor : MonoBehaviour, IGameEventListener<GameLevelEvent>
    {
        [SerializeField] private LevelDoorHud _hud = default;
        [SerializeField] private Transform _art = default;
        [SerializeField] private GameObject _artEnable = default;
        [SerializeField] private float _animationDuration = default;
        [SerializeField] private float _deltaPosition = -1;

        private bool _isSelectable = false;

        #region Unity events

        private void OnEnable()
        {
            this.EventStartListening<GameLevelEvent>();
        }

        private void OnDisable()
        {
            this.EventStopListening<GameLevelEvent>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_isSelectable) return;

            // Check if HUD was already shown
            if (_hud.IsOpen) return;

            if (!other.gameObject.tag.Equals("Player")) return;

            // Show HUD
            _hud.Show();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_isSelectable) return;

            if (!other.gameObject.tag.Equals("Player")) return;

            // Hide HUD
            _hud.Hide();
        }

        #endregion

        #region GameEventListener<GameLevelEvent> implementation

        public virtual void OnGameEvent(GameLevelEvent eventData)
        {
            switch (eventData.EventType)
            {
                case GameLevelEventType.LevelLoaded:
                    Init();
                    break;

                case GameLevelEventType.LevelCompleted:
                    LevelCompleted();
                    break;
            }
        }

        #endregion

        #region Private methods

        private void Init()
        {
            // TODO: load information related with what it should represent like:
            //   - next chamber
            //   - rewards

            // Mark it as not selectable
            _isSelectable = false;

            _artEnable.Toggle(false);

            _hud.Setup(Open);
        }

        private void LevelCompleted()
        {
            _isSelectable = true;

            _artEnable.Toggle(true);
        }

        private void Open()
        {
            _isSelectable = false;

            StartCoroutine(AnimateDoorOpening());
        }

        private IEnumerator AnimateDoorOpening()
        {
            var startPosition = _art.localPosition;
            var endPosition = startPosition + Vector3.up * _deltaPosition;

            var t = 0f;
            while (t < _animationDuration)
            {
                _art.localPosition = Vector3.Lerp(startPosition, endPosition, t / _animationDuration);

                t += Time.deltaTime;

                yield return null;
            }

            _art.localPosition = endPosition;
        }

        #endregion
    }
}