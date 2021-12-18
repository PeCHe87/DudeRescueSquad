using UnityEngine;

namespace DudeResqueSquad
{
    public class DestructiblesManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera = null;
        [SerializeField] private GameObject[] _damageables = null;
        [SerializeField] private GameObject _healthBar = null;

        private void Start()
        {
            int amount = _damageables.Length;

            for (int i = 0; i < amount; i++)
            {
                var damageable = _damageables[i];

                if (damageable == null)
                    continue;

                var damageableComponent = damageable.GetComponent<DamageableProp>();
                damageableComponent.Init();

                var healthBar = damageable.GetComponentInChildren<FloatingHealthBar>();

                if (healthBar == null)
                    continue;

                healthBar.Init(_camera, damageable.transform, 0);
            }
        }
    }
}