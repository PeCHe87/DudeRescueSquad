using UnityEngine;

namespace DudeResqueSquad
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _trailEffect = null;

        public void Attack()
        {
            _trailEffect.Play();
        }
    }
}