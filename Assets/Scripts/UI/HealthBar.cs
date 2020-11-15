using System;
using UnityEngine;

namespace DudeResqueSquad
{
    /// <summary>
    /// Displays a configurable health bar for any object with a IDamageable as a parent
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Camera _camera;

        #endregion

        #region Private properties

        private MaterialPropertyBlock _matBlock;
        private MeshRenderer _meshRenderer;
        private IDamageable _damageable;

        #endregion

        #region Private methods

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _matBlock = new MaterialPropertyBlock();
           
            // Get the damageable parent we're attached to
            _damageable = GetComponentInParent<IDamageable>();

            if (_damageable != null)
            {
                _damageable.OnTakeDamage += UpdateHealth;
                _damageable.OnDied += EntityDead;
            }

            _meshRenderer.enabled = false;
        }

        private void OnDestroy()
        {
            if (_damageable != null)
            {
                _damageable.OnTakeDamage -= UpdateHealth;
                _damageable.OnDied -= EntityDead;
            }
        }

        private void Update()
        {
            AlignCamera();
        }

        private void UpdateHealth(object sender, CustomEventArgs.DamageEventArgs e)
        {
            // Only display on partial health
            UpdateParams();

            _meshRenderer.enabled = true;
        }

        private void EntityDead(object sender, CustomEventArgs.EntityDeadEventArgs e)
        {
            _meshRenderer.enabled = false;
            UpdateParams();
            Destroy(gameObject, 0.25f);

            Debug.Log($"Entity <b>Dead</b>!");
        }

        private void UpdateParams()
        {
            _meshRenderer.GetPropertyBlock(_matBlock);
            _matBlock.SetFloat("_Fill", _damageable.Health / (float)_damageable.MaxHealth);
            _meshRenderer.SetPropertyBlock(_matBlock);
        }

        private void AlignCamera()
        {
            if (_camera != null)
            {
                var camXform = _camera.transform;
                var forward = transform.position - camXform.position;
                forward.Normalize();
                var up = Vector3.Cross(forward, camXform.right);
                transform.rotation = Quaternion.LookRotation(forward, up);
            }
        }

        #endregion

        #region Public methods

        public void SetCamera(Camera cam)
        {
            // Cache Camera
            _camera = cam;
        }

        #endregion
    }
}