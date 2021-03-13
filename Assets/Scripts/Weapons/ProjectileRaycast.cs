using UnityEngine;

namespace DudeResqueSquad.Weapons
{
    /*
     * This class represents a projectile that is controlled based on the formula to detect where it should be based on current time alive since fired
     * Formula to get position in time: [initial position] + [initial velocity * current time] + [0.5 * gravity * time^2]
     */
    public class ProjectileRaycast
    {
        public float CurrentTime;
        public float LifeTime;
        public LayerMask TargetLayerMask;
        public string EntityUID;
        public float Damage;
        public GameObject HitVFX;

        private readonly GameObject bullet;
        private readonly Vector3 initialPosition;
        private readonly Vector3 initialVelocity;
        private readonly float dropSpeed;
        private TrailRenderer trail;

        public ProjectileRaycast(GameObject bullet, Vector3 position, Vector3 velocity, float dropSpeed, float timeLife, TrailRenderer trail)
        {
            this.bullet = bullet;
            this.initialPosition = position;
            this.initialVelocity = velocity;
            this.dropSpeed = dropSpeed;
            this.trail = trail;

            this.LifeTime = timeLife;
            
            this.CurrentTime = 0;
        }
        
        public Vector3 GetPosition()
        {
            var gravity = Vector3.down * dropSpeed;
                
            return initialPosition + (initialVelocity * CurrentTime) + (gravity * (0.5f * CurrentTime * CurrentTime));
        }
        
        public Vector3 GetPositionAtTime(float time)
        {
            var gravity = Vector3.down * dropSpeed;
                
            return initialPosition + (initialVelocity * time) + (gravity * (0.5f * time * time));
        }

        public void UpdateBulletPosition(Vector3 position)
        {
            if (this.bullet != null)
            {
                this.bullet.transform.position = position;
            }
        }

        public GameObject GetVisualRepresentation()
        {
           return this.bullet;
        }
    }
}
