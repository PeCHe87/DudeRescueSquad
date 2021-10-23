using DudeRescueSquad.Global;
using System.Collections.Generic;
using UnityEngine;
using static DudeRescueSquad.Core.Weapons.Weapon;

namespace DudeResqueSquad.Weapons
{
    public class ProjectilesContainer : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Transform _container = null;
        [SerializeField] protected bool _canDebugProjectile = false;

        #endregion
        
        #region Private properties
        
        private readonly List<ProjectileRaycast> _projectiles = new List<ProjectileRaycast>();
        private Ray _ray;
        private RaycastHit _hitInfo;
        private static ProjectilesContainer _instance = null;

        #endregion

        public static ProjectilesContainer Instance { get => _instance; private set => _instance = value; }

        #region Private methods

        private void Awake()
        {
            if (_instance == null) _instance = this;

            GameEvents.OnSpawnProjectile += SpawnProjectile;
        }

        private void OnDestroy()
        {
            GameEvents.OnSpawnProjectile -= SpawnProjectile;
        }

        private void LateUpdate()
        {
            UpdateProjectiles(Time.deltaTime);

            DestroyProjectiles();
        }
        
        private void UpdateProjectiles(float deltaTime)
        {
            int amount = _projectiles.Count;

            for (int i = 0; i < amount; i++)
            {
                var projectile = _projectiles[i];
                var start = projectile.GetPosition();
                
                projectile.CurrentTime += deltaTime;
                
                var end = projectile.GetPosition();

                RaycastSegment(start, end, projectile);

                projectile.UpdateBulletPosition(end);
            }
        }
        
        private void RaycastSegment(Vector3 start, Vector3 end, ProjectileRaycast projectile)
        {
            float distance = (end - start).magnitude;
            
            _ray.origin = start;
            _ray.direction = end - start;

            if (_canDebugProjectile)
            {
                Debug.DrawLine(start, end, Color.blue, 1.0f);
            }

            // Detect target hit
            if (Physics.Raycast(_ray, out _hitInfo, distance, projectile.TargetLayerMask))
            {
                DestroyBulletVisualRepresentation(projectile);

                // Show hit VFX
                if (projectile.HitVFX != null)
                {
                    ShowHitVFX(_hitInfo, projectile.HitVFX);
                }

                // Mark this projectile as ready to be destroyed
                projectile.CurrentTime = projectile.LifeTime;

                if (_canDebugProjectile)
                {
                    Debug.Log($"<color=orange>Projectile</color> from {projectile.EntityUID} impacts {_hitInfo.collider.name}");
                }
                

                // Generate damage
                GenerateDamage(projectile.Damage);
            }
        }
        
        /// <summary>
        /// Destroy bullet visual representation
        /// </summary>
        /// <param name="projectile"></param>
        private void DestroyBulletVisualRepresentation(ProjectileRaycast projectile)
        {
            var bullet = projectile.GetVisualRepresentation();

            SimplePool.Despawn(bullet, false);     //Destroy(bullet);
        }

        /// <summary>
        /// Generates and amount of damage in the IDamageable component, if it exists
        /// </summary>
        /// <param name="damage"></param>
        private void GenerateDamage(float damage)
        {
            var damageable = _hitInfo.collider.GetComponent<IDamageable>();

            Debug.Log($"[ProjectilesContainer] GenerateDamage to {damageable}, damage: {damage}");

            damageable?.TakeDamage(damage);
        }

        /// <summary>
        /// Create a new hit VFX based on hit point
        /// </summary>
        /// <param name="hitInfo"></param>
        private void ShowHitVFX(RaycastHit hitInfo, GameObject hitVFXPrefab)
        {
            if (hitVFXPrefab == null) return;

            var hitVFX = Instantiate(hitVFXPrefab).transform;

            hitVFX.position = hitInfo.point;
            hitVFX.forward = hitInfo.normal;
        }

        /// <summary>
        /// Destroy all projectiles that their current time is bigger than their lifetime
        /// </summary>
        private void DestroyProjectiles()
        {
            // Remove all projectiles that its current time is over than the lifetime and destroy its visual representation
            _projectiles.RemoveAll(projectile =>
            {
                bool removable = projectile.CurrentTime >= projectile.LifeTime;

                if (removable)
                {
                    DestroyBulletVisualRepresentation(projectile);
                }
                
                return removable;
            });
        }
        
        #endregion
        
        #region Public methods

        public void SpawnProjectile(object sender, CustomEventArgs.SpawnProjectileEventArgs e)    
        {
            GameObject bulletRepresentation = SimplePool.Spawn(e.prefab, _container);

            bulletRepresentation.transform.position = e.positionInitial;
            bulletRepresentation.transform.rotation = e.initialRotation;
            var projectile = new ProjectileRaycast(bulletRepresentation, e.positionInitial, e.velocity, e.dropSpeed, e.lifeTime, null);
            projectile.Damage = e.damage;
            projectile.EntityUID = e.entityOwnerUID;
            projectile.HitVFX = e.prefabHitVFX;
            projectile.TargetLayerMask = e.layerMask;
            
            // Add projectile
            _projectiles.Add(projectile);

            if (_canDebugProjectile)
            {
                Debug.DrawLine(e.positionInitial, projectile.GetPositionAtTime(e.lifeTime), Color.magenta, 1);
            }
        }

        public void SpawnSimpleProjectile(DudeRescueSquad.Core.Weapons.SimpleProjectileData data, Vector3 position, Vector3 velocity, Quaternion rotation)
        {
            GameObject bulletRepresentation = SimplePool.Spawn(data.prefab, _container);

            bulletRepresentation.transform.position = position;
            bulletRepresentation.transform.rotation = rotation;

            // TODO: this should be replaced for another pooling and call to Setup each time it is spawn
            var projectile = new ProjectileRaycast(bulletRepresentation, position, velocity, 0, data.lifetime, null);
            projectile.Damage = data.damage;
            projectile.EntityUID = string.Empty;
            projectile.HitVFX = data.hitVfx;
            projectile.TargetLayerMask = data.layerMask;

            // Add projectile
            _projectiles.Add(projectile);

            if (_canDebugProjectile)
            {
                Debug.DrawLine(position, projectile.GetPositionAtTime(data.lifetime), Color.magenta, 1);
            }
        }

        #endregion
    }
}