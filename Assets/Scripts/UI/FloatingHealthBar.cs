using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DudeResqueSquad
{
    public class FloatingHealthBar : MonoBehaviour
    {
        [SerializeField] private Transform _target = null;
        [SerializeField] private float _pivotOffset = 0;
        [SerializeField] private Camera _camera = null;
        [SerializeField] private Image _imgFill = null;
        [SerializeField] private float _speedToUpdateHealth = 0.5f;

        private Transform _transform = null;
        private Transform _cameraTransform = null;
        private IDamageable _damageable = null;

        private readonly Vector3 _up = Vector3.up;
        private readonly Vector3 _back = Vector3.back;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnDestroy()
        {
            if (_damageable == null)
                return;

            _damageable.OnTakeDamage -= TakeDamage;
            _damageable.OnDied -= Die;
        }

        private void LateUpdate()
        {
            if (_camera == null)
                return;

            if (_target == null)
                return;

            //_transform.position = _camera.ScreenToWorldPoint(_target.position + _up * _pivotOffset);
            //_transform.LookAt(_camera.transform);
            //_transform.Rotate(0, 180, 0);

            _transform.LookAt(_transform.position + _cameraTransform.rotation * _back, _cameraTransform.rotation * _up);
        }

        private void TakeDamage(object sender, CustomEventArgs.DamageEventArgs e)
        {
            float perc = _damageable.Health / _damageable.MaxHealth;

            StartCoroutine(UpdateHealthProgress(perc));
        }

        private void Die(object sender, EventArgs e)
        {
            float perc = _damageable.Health / _damageable.MaxHealth;

            StartCoroutine(UpdateHealthProgress(perc));
        }

        private IEnumerator UpdateHealthProgress(float perc)
        {
            float preChangePerc = _imgFill.fillAmount;

            float elapsed = 0f;

            while (elapsed < _speedToUpdateHealth)
            {
                elapsed += Time.deltaTime;

                _imgFill.fillAmount = Mathf.Lerp(preChangePerc, perc, elapsed / _speedToUpdateHealth);

                yield return null;
            }

            _imgFill.fillAmount = perc;

            if (perc == 0)
                HideProgressBar();
        }

        private void HideProgressBar()
        {
            Destroy(gameObject);
        }

        public void Init(Camera camera, Transform target, float offset)
        {
            _target = target;
            _camera = camera;
            _pivotOffset = offset;
            _cameraTransform = _camera.transform;

            _damageable = target.GetComponent<IDamageable>();

            if (_damageable == null)
                return;

            _damageable.OnTakeDamage += TakeDamage;
            _damageable.OnDied += Die;

            _imgFill.fillAmount = _damageable.Health / _damageable.MaxHealth;
        }
    }
}