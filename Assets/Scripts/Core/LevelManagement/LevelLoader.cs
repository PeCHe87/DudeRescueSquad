using DudeRescueSquad.Core.Characters;
using UnityEngine;

namespace DudeRescueSquad.Core.LevelManagement
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Character _character = default;

        private void Start()
        {
            GameLevelEvent.Trigger(GameLevelEventType.LevelLoaded, _character);
        }

        private void OnDestroy()
        {
            GameLevelEvent.Trigger(GameLevelEventType.LevelUnloaded);
        }
    }
}