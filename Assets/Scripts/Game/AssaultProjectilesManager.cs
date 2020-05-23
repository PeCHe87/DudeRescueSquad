using System;
using UnityEngine;
using static EffectActor;

namespace DudeResqueSquad
{
    public class AssaultProjectilesManager : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Transform spawnLocator;
        [SerializeField] private Transform spawnLocatorMuzzleFlare;
        [SerializeField] private Transform shellLocator;
        [SerializeField] private Animator recoilAnimator;

        [Header("Shotgun spawn origins")]
        [SerializeField] private Transform[] shotgunLocator;

        #endregion

        private bool firing;
        private PlayerAttackController _attackController = null;
        private ItemWeaponData _currentWeapon = null;

        #region Private methods

        private void Awake()
        {
            _attackController = GetComponent<PlayerAttackController>();

            if (_attackController == null)
                return;

            _attackController.OnShot += Shot;
        }

        private void OnDestroy()
        {
            if (_attackController == null)
                return;

            _attackController.OnShot -= Shot;
        }

        private void Shot(CustomEventArgs.ShotEventArgs e)
        {
            // Check assault weapon equipped
            if (e.weaponData.AttackType == Enums.WeaponAttackType.ASSAULT_1_HAND || 
                e.weaponData.AttackType == Enums.WeaponAttackType.ASSAULT_2_HANDS)
            {
                // Fire projectile related with assault weapon equipped
                _currentWeapon = e.weaponData;

                // Spawn new projectile
                Invoke("Fire", e.weaponData.DelayFireEffect);
            }
        }

        public void Fire()
        {
            /*if (CameraShake)
            {
                CameraShakeCaller.ShakeCamera();
            }*/

            var projectileConfig = _currentWeapon.ProjectileConfiguration;

            var muzzle = Instantiate(projectileConfig.muzzleflare, spawnLocatorMuzzleFlare.position, spawnLocatorMuzzleFlare.rotation);
            muzzle.transform.localScale = Vector3.one * 0.5f;

            if (projectileConfig.hasShells)
            {
                Instantiate(projectileConfig.shellPrefab, shellLocator.position, shellLocator.rotation);
            }

            Rigidbody rocketInstance;

            rocketInstance = Instantiate(projectileConfig.bombPrefab, spawnLocator.position, spawnLocator.rotation) as Rigidbody;

            // Set projectile damage
            var projectile = rocketInstance.GetComponent<ExplodingProjectile>();

            if (projectile != null)
            {
                projectile.Damage = _currentWeapon.Damage;
                projectile.explodeOnTimer = true;
            }

            rocketInstance.transform.localScale = Vector3.one * 0.5f;

            rocketInstance.AddForce(spawnLocator.forward * UnityEngine.Random.Range(projectileConfig.min, projectileConfig.max));

            if (projectileConfig.shotgunBehavior)
            {
                for (int i = 0; i < projectileConfig.shotgunPellets; i++)
                {
                    Rigidbody rocketInstanceShotgun;

                    rocketInstanceShotgun = Instantiate(projectileConfig.bombPrefab, shotgunLocator[i].position, shotgunLocator[i].rotation) as Rigidbody;

                    rocketInstanceShotgun.transform.localScale = Vector3.one * 0.5f;

                    rocketInstanceShotgun.AddForce(shotgunLocator[i].forward * UnityEngine.Random.Range(projectileConfig.min, projectileConfig.max));
                }
            }
        }

        #endregion
    }
}