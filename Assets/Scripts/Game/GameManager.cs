using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class GameManager : MonoBehaviour
    {
        #region Events

        public Action OnGameStarted;
        public Action<Enums.KeyType, PlayerData> OnPlayerCollectKey;
        public Action<ItemWeaponData, PlayerData> OnPlayerCollectWeapon;
        public Action<string, int, PlayerData> OnDoorOpened;

        #endregion

        public static GameManager Instance;

        [SerializeField] private Camera _mainCamera = null;
        [SerializeField] private Camera _minimapCamera = null;

        public Camera MainCamera { get => _mainCamera; }
        public Camera MinimapCamera { get => _minimapCamera; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
}