using System;
using UnityEngine;
using static EffectActor;

namespace DudeResqueSquad
{
    public class AssaultProjectilesManager : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private PlayerData _data = null;
        [SerializeField] private Transform spawnLocator;
        [SerializeField] private Transform spawnLocatorMuzzleFlare;
        [SerializeField] private Transform shellLocator;
        [SerializeField] private Animator recoilAnimator;

        [Header("Shotgun spawn origins")]
        [SerializeField] private Transform[] shotgunLocator;

        #endregion

        #region Private properties

        private bool firing;
        private PlayerAttackController _attackController = null;
        private ItemWeaponData _currentWeapon = null;
        private bool _muzzleWasCreated = false;
        private GameObject _muzzle = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            _attackController = GetComponent<PlayerAttackController>();

            if (_attackController == null)
                return;

            _attackController.OnShot += Shot;

            GameEvents.OnCollectItem += CollectItem;
            GameEvents.OnStopAction += StopAction;
        }

        private void OnDestroy()
        {
            if (_attackController == null)
                return;

            _attackController.OnShot -= Shot;

            GameEvents.OnCollectItem -= CollectItem;
            GameEvents.OnStopAction -= StopAction;
        }

        private void StopAction(object sender, EventArgs e)
        {
            _muzzleWasCreated = false;

            if (_muzzle != null)
                Destroy(_muzzle);

            Debug.Log(" ---- Destroy muzzle (?)");
        }

        private void CollectItem(object sender, CustomEventArgs.CollectItemEventArgs e)
        {
            var item = e.item;

            if (item.Type != Enums.ItemType.WEAPON)
                return;

            var playerUID = e.playerUID;

            if (!_data.UID.Equals(playerUID))
                return;

            var weapon = (ItemWeaponData)item;

            // Change projectile locator position
            spawnLocator.localPosition = weapon.PositionProjectileSpawner;

            // Change muzzle position
            spawnLocatorMuzzleFlare.localPosition = weapon.PositionMuzzleSpawner;
        }

        private void Shot(CustomEventArgs.PlayerAttackEventArgs e)
        {
            // Check assault weapon equipped
            if (e.weaponData.AttackType == Enums.WeaponAttackType.ASSAULT_1_HAND || 
                e.weaponData.AttackType == Enums.WeaponAttackType.ASSAULT_2_HANDS ||
                e.weaponData.AttackType == Enums.WeaponAttackType.ASSAULT_RIFLE)
            {
                // Fire projectile related with assault weapon equipped
                _currentWeapon = e.weaponData;

                spawnLocator.localPosition = _currentWeapon.PositionProjectileSpawner;
                spawnLocatorMuzzleFlare.localPosition = _currentWeapon.PositionMuzzleSpawner;

                // Spawn new projectile
                Fire();
            }
        }

        public void Fire()
        {
            /*if (CameraShake)
            {
                CameraShakeCaller.ShakeCamera();
            }*/

            var projectileConfig = _currentWeapon.ProjectileConfiguration;

            if (_currentWeapon.AutoFire)
            {
                if (!_muzzleWasCreated)
                {
                    _muzzleWasCreated = true;

                    _muzzle = Instantiate(projectileConfig.muzzleflare, spawnLocatorMuzzleFlare.position, spawnLocatorMuzzleFlare.rotation);
                    _muzzle.transform.localScale = Vector3.one * 0.5f;

                    var destroyBehavior = _muzzle.GetComponent<destroyMe>();

                    if (destroyBehavior != null)
                        destroyBehavior.enabled = false;

                    Debug.Log(" ---- Create muzzle");
                }
                else
                {
                    _muzzle.GetComponent<ParticleSystem>().Play();
                    _muzzle.transform.position = spawnLocatorMuzzleFlare.position;
                    _muzzle.transform.rotation = spawnLocatorMuzzleFlare.rotation;
                }
            }
            else
            {
                var muzzle = Instantiate(projectileConfig.muzzleflare, spawnLocatorMuzzleFlare.position, spawnLocatorMuzzleFlare.rotation);
                muzzle.transform.localScale = Vector3.one * 0.5f;
            }

            if (projectileConfig.hasShells)
            {
                Instantiate(projectileConfig.shellPrefab, shellLocator.position, shellLocator.rotation);
            }

            Rigidbody rocketInstance;

            rocketInstance = Instantiate(projectileConfig.bombPrefab, spawnLocator.position, spawnLocator.rotation) as Rigidbody;

            // Set projectile damage
            SetProjectileDamage(_currentWeapon.Damage, rocketInstance.gameObject);

            rocketInstance.transform.localScale = Vector3.one * 0.5f;

            rocketInstance.AddForce(spawnLocator.forward * UnityEngine.Random.Range(projectileConfig.min, projectileConfig.max));

            if (projectileConfig.shotgunBehavior)
            {
                for (int i = 0; i < projectileConfig.shotgunPellets; i++)
                {
                    Rigidbody rocketInstanceShotgun;

                    rocketInstanceShotgun = Instantiate(projectileConfig.bombPrefab, shotgunLocator[i].position, shotgunLocator[i].rotation) as Rigidbody;

                    // Set projectile damage
                    SetProjectileDamage(_currentWeapon.Damage, rocketInstanceShotgun.gameObject);

                    rocketInstanceShotgun.transform.localScale = Vector3.one * 0.5f;

                    rocketInstanceShotgun.AddForce(shotgunLocator[i].forward * UnityEngine.Random.Range(projectileConfig.min, projectileConfig.max));
                }
            }
        }

        private void SetProjectileDamage(float damage, GameObject rocketInstance)
        {
            var projectile = rocketInstance.GetComponent<ExplodingProjectile>();

            if (projectile != null)
            {
                projectile.Damage = _currentWeapon.Damage;
                projectile.explodeOnTimer = true;
                //projectile.explosionTimer = _currentWeapon.AttackDelayTime * 2;
                projectile.explosionTimer = _currentWeapon.projectileLifetime;
            }
        }

        #endregion
    }
}