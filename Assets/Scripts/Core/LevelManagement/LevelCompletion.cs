using DudeRescueSquad.Core.Events;
using DudeResqueSquad;
using UnityEngine;

namespace DudeRescueSquad.Core.LevelManagement
{
    public class LevelCompletion : MonoBehaviour, IGameEventListener<GameLevelEvent>
    {
        [SerializeField] private Entity[] _enemies = default;

        private int _amountEnemies = default;
        private bool _wasCompleted = false;

        #region Unity events

        private void OnEnable()
        {
            this.EventStartListening<GameLevelEvent>();
        }

        private void OnDisable()
        {
            this.EventStopListening<GameLevelEvent>();
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

                case GameLevelEventType.EnemyDied:
                    EnemyDied();
                    break;
            }
        }

        #endregion

        #region Private methods

        private void Init()
        {
            _amountEnemies = _enemies.Length;
        }

        private void EnemyDied()
        {
            if (_wasCompleted) return;

            _amountEnemies--;

            if (_amountEnemies == 0)
            {
                Debug.LogError("<b>LevelCompletion</b> => <color=green>Level Completed</color>");

                _wasCompleted = true;

                GameLevelEvent.Trigger(GameLevelEventType.LevelCompleted);
            }
        }

        #endregion
    }
}