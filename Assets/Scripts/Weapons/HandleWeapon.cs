using UnityEngine;

namespace DudeResqueSquad
{

    public class HandleWeapon : MonoBehaviour
    {
        #region Inspector properties

        [Header("Binding")]
        [SerializeField] private bool _applyIK = false;
        [Tooltip("The position the weapon will be attached to. If left blank, will be this.transform.")]
        [SerializeField] private Transform WeaponAttachment;
        [Tooltip("The position from which projectiles will be spawned.")]
        [SerializeField] private Transform ProjectileSpawn;

        #endregion

        #region Private properties

        private Weapon _currentWeapon = null;
        private WeaponIK _weaponIK = null;

        #endregion

        #region Private methods

        private void Start()
        {
            _weaponIK = GetComponentInChildren<WeaponIK>();
        }

        private void HandleWeaponIK()
        {
            if (_weaponIK != null)
                _weaponIK.SetHandles(_currentWeapon.LeftHandHandle, _currentWeapon.RightHandHandle);
        }

        private void InstantiateWeapon(Weapon newWeapon, string weaponID)
        {
            _currentWeapon = (Weapon)Instantiate(newWeapon, WeaponAttachment.transform.position, WeaponAttachment.transform.rotation);

            _currentWeapon.transform.parent = WeaponAttachment.transform;
            _currentWeapon.transform.localPosition = Vector3.zero;
            _currentWeapon.transform.localRotation = Quaternion.identity;

            // We handle (optional) inverse kinematics (IK) 
            if (_applyIK)
                HandleWeaponIK();
        }

        /// <summary>
        /// Causes the character to stop shooting
        /// </summary>
        private void ShootStop()
        {
            // TODO
        }

        #endregion

        #region Public methods

        public void ChangeWeapon(Weapon newWeapon, string weaponID)
        {
            // If the character already has a weapon, we make it stop shooting
            if (_currentWeapon != null)
            {
                // TODO: _currentWeapon.TurnWeaponOff();

                ShootStop();

                Destroy(_currentWeapon.gameObject);
            }

            if (newWeapon != null)
                InstantiateWeapon(newWeapon, weaponID);
        }

        #endregion
    }
}