using UnityEngine;

namespace DudeResqueSquad
{
    public class EntityTextDamage : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Camera _camera = null;
        [SerializeField] private Transform _pivot = null;
        [SerializeField] private int _fontSize = 10;

        #endregion

        #region Private properties

        private IDamageable _damageable = null;
        private Vector3 _pivotPosition = Vector3.zero;

        #endregion

        #region Private methods

        private void Awake()
        {
            _pivotPosition = _pivot.localPosition;

            _damageable = GetComponent<IDamageable>();

            if (_damageable != null)
            {
                _damageable.OnTakeDamage += ShowDamage;
            }
        }

        private void OnDestroy()
        {
            if (_damageable != null)
            {
                _damageable.OnTakeDamage -= ShowDamage;
            }
        }

        private void ShowDamage(object sender, CustomEventArgs.DamageEventArgs e)
        {
            Vector3 randomPosition = UnityEngine.Random.insideUnitCircle;
            float randomFactor = RandomGames.Utils.Utils.GetRandomNumberBetweenTwoNumbers(-0.5f, 0.5f);

            var position = _pivot.position + (randomFactor * randomPosition);

            var damagePopup = DamagePopup.Create(position, Mathf.FloorToInt(e.damage), false);

            damagePopup.SetFontSize(_fontSize);
            //damagePopup.SetCameraOrientation(_camera);
        }

        #endregion
    }
}