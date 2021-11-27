using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DudeResqueSquad
{
    public class PlayerAttackController : MonoBehaviour
    {
        #region Events

        public Action<CustomEventArgs.PlayerAttackEventArgs> OnShot;
        public Action<CustomEventArgs.PlayerAttackEventArgs> OnMeleeAttack;
        public Action<CustomEventArgs.PlayerAttackEventArgs> OnStartReloading;
        public Action<CustomEventArgs.PlayerAttackEventArgs> OnFinishReloading;
        public Action OnCancelReloading;
        public Action OnReloading;
        public Action OnNoBullets;

        #endregion

        #region Inspector properties

        [Tooltip("Main cone of vision")]
        [SerializeField] private FieldOfView _fov = null;
        [Tooltip("Secondary cone of vision")]
        [SerializeField] private FieldOfView _fovSecondary = null;
        [SerializeField] private ParticleSystem _meleeTrailEffect = null;
        [SerializeField] private float _delayToStartReloading = 1;
        [SerializeField] private bool _canDebug = false;

        #endregion

        #region Private properties

        private ItemWeaponData _currentItemEquipped = null;
        private Character _character = null;
        private ICharacterMovement _movement = null;
        private Animator _animator = null;
        private int _currentAmountAttacks = 0;
        private int _currentAttack = 0;
        private bool _attackInProgress = false;
        private List<IDamageable> _cacheTargets = null;
        private bool _isReloading = false;
        private bool _autoFireActive = false;
        private bool _usingMainFieldOfView = true;

        #endregion

        #region Private methods

        private void Awake()
        {
            _character = GetComponent<Character>();

            if (_character != null)
                _character.OnEquipItem += EquipItem;

            _movement = GetComponent<ICharacterMovement>();

            if (_movement == null)
                return;

            _movement.OnDoAction += Attack;
            _movement.OnStartAction += StartAction;

            GameEvents.OnStopAction += StopAction;

            _cacheTargets = new List<IDamageable>();
        }

        private void Update()
        {
            if (_currentItemEquipped == null)
                return;

            // If there is a current item and it is reloading then update its remaining time to finish the reloading.
            if (_currentItemEquipped.IsReloading)
            {
                _currentItemEquipped.RemainingReloadTime -= Time.deltaTime;

                if (_currentItemEquipped.RemainingReloadTime <= 0)
                {
                    // Update current bullets magazine
                    _currentItemEquipped.CurrentBulletsMagazine = (_currentItemEquipped.InfiniteBullets) ? _currentItemEquipped.BulletsMagazine : _currentItemEquipped.BulletsToReload;

                    // Update current bullets amount
                    _currentItemEquipped.CurrentBulletsAmount -= _currentItemEquipped.BulletsToReload;

                    // Clean data
                    _currentItemEquipped.IsReloading = false;
                    _currentItemEquipped.BulletsToReload = 0;
                    _currentItemEquipped.RemainingReloadTime = 0;

                    Debug.Log("RELOADING is finished");

                    OnFinishReloading?.Invoke(new CustomEventArgs.PlayerAttackEventArgs(_currentItemEquipped, _character.Data.UID));
                }
            }
        }

        private void OnDestroy()
        {
            if (_character != null)
                _character.OnEquipItem -= EquipItem;

            if (_movement != null)
            {
                _movement.OnDoAction -= Attack;
                _movement.OnStartAction -= StartAction;
            }

            GameEvents.OnStopAction -= StopAction;
        }

        private void StartAction(object sender, EventArgs e)
        {
            // Logic to start doing action until release the button
            if (_fov == null)
                return;

            if (_character == null)
                return;

            if (_currentItemEquipped == null)
                return;

            if (_currentItemEquipped.IsReloading)
            {
                Debug.Log("Is RELOADING!");
                OnReloading?.Invoke();
                return;
            }

            _autoFireActive = true;

            _attackInProgress = true;

            ProcessAssaultAttack();
        }

        private void StopAction(object sender, EventArgs e)
        {
            // Logic to stop doing current action
            ResetAttackState();
        }

        private void ProcessAssaultAttack()
        {
            // If attack wan't canceled the skip logic
            if (!_attackInProgress)
                return;

            // Check magazine current bullets remaining
            if (_currentItemEquipped.CurrentBulletsMagazine == 0)
            {
                // Check amount of remaining bullets
                if (_currentItemEquipped.CurrentBulletsAmount == 0)
                {
                    Debug.Log("<b>STOP ACTION</b> - No more BULLETS");

                    GameEvents.OnStopAction?.Invoke(this, EventArgs.Empty);

                    OnNoBullets?.Invoke();
                }

                return;
            }

            // Check weapon equipped to cause damage based on its configuration
            if (_character.State.CurrentState != Enums.CharacterStates.ATTACKING)
                _character.State.SetState(Enums.CharacterStates.ATTACKING);

            // Rotates toward the nearest target
            RotateTowardsNearestTarget();

            // Consume one bullet
            _currentItemEquipped.CurrentBulletsMagazine--;

            // Check if it should start to reload bullets
            if (_currentItemEquipped.CurrentBulletsMagazine == 0)
            {
                if (_currentItemEquipped.InfiniteBullets || _currentItemEquipped.CurrentBulletsAmount > 0)
                    Invoke("ReloadBullets", _delayToStartReloading);
            }

            // Delay to rotate again if target was moving
            Invoke("RotateTowardsNearestTarget", _currentItemEquipped.DelayFireEffect - 0.05f);

            OnShot?.Invoke(new CustomEventArgs.PlayerAttackEventArgs(_currentItemEquipped, _character.Data.UID));

            //Debug.Log($"<color=blue>ProcessAssaultAttack</color> - Bullets: <i>{_currentItemEquipped.CurrentBulletsMagazine}/{_currentItemEquipped.CurrentBulletsAmount}</i>");

            // If auto fire then invoke again after fire rate (delay between bullets)
            if (_currentItemEquipped.CurrentBulletsMagazine == 0)
            {
                //Debug.Log("<b>STOP ACTION</b> - No more BULLETS");

                GameEvents.OnStopAction?.Invoke(this, EventArgs.Empty);

                OnNoBullets?.Invoke();
            }
            else if (_autoFireActive)
                Invoke("ProcessAssaultAttack", _currentItemEquipped.DelayBetweenBullets);
        }

        private void EquipItem()
        {
            // Update values for Field of View based on current weapon configuration
            if (_fov == null)
                return;

            _currentItemEquipped = _character.Data.CurrentWeaponEquipped;

            if (_currentItemEquipped == null)
            {
                _fov.enabled = false;

                return;
            }

            _fovSecondary.Radius = _currentItemEquipped.DetectionAreaRadius;
            _fovSecondary.ViewAngle = _currentItemEquipped.AngleAttackArea;
            _fovSecondary.enabled = true;
        }

        private void Attack(object sender, CustomEventArgs.TouchEventArgs e)
        {
            if (_fov == null)
                return;

            if (_character == null)
                return;

            if (_currentItemEquipped == null)
                return;

            if (_character.State.CurrentState == Enums.CharacterStates.ATTACKING)
                _currentAmountAttacks++;

            if (_currentAttack > 0)
                Debug.Log($"Attack! Current Attack: {_currentAttack}, total amount: {_currentAmountAttacks}");

            if (_attackInProgress)
                return;

            if (_canDebug)
                Debug.Log("<color=green>Attack!</color>");

            // If it is an assault attack then communicate that to anyone interested
            if (_currentItemEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_1_HAND ||
                _currentItemEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_2_HANDS ||
                _currentItemEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_RIFLE)
            {
                if (_currentItemEquipped.IsReloading)
                {
                    Debug.Log("Is RELOADING!");
                    OnReloading?.Invoke();
                    return;
                }

                // Check magazine current bullets remaining
                if (_currentItemEquipped.CurrentBulletsMagazine == 0)
                {
                    // Check amount of remaining bullets
                    if (_currentItemEquipped.CurrentBulletsAmount == 0)
                    {
                        Debug.Log("No more BULLETS");
                        OnNoBullets?.Invoke();
                    }

                    return;
                }

                // Check weapon equipped to cause damage based on its configuration
                _character.State.SetState(Enums.CharacterStates.ATTACKING);

                // Rotates toward the nearest target
                RotateTowardsNearestTarget();

                // Consume one bullet
                _currentItemEquipped.CurrentBulletsMagazine--;

                // Check if it should start to reload bullets
                if (_currentItemEquipped.CurrentBulletsAmount > 0 && _currentItemEquipped.CurrentBulletsMagazine == 0)
                {
                    //_currentItemEquipped.IsReloading = true;

                    Invoke("ReloadBullets", _delayToStartReloading);
                }

                // Delay to rotate again if target was moving
                Invoke("RotateTowardsNearestTarget", _currentItemEquipped.DelayFireEffect - 0.05f);
            }
            else
            {
                // Check durability
                if (_currentItemEquipped.CurrentDurability == 0)
                {
                    Debug.Log("No more DURABILITY");

                    return;
                }

                // Check weapon equipped to cause damage based on its configuration
                _character.State.SetState(Enums.CharacterStates.ATTACKING);

                // Rotates toward the nearest target
                RotateTowardsNearestTarget();

                _meleeTrailEffect?.Play();
            }

            _attackInProgress = true;

            _currentAttack = 1;

            _currentAmountAttacks = 1;

            float delayAttackTime = _currentItemEquipped.AttackDelayTime;

            Invoke("AttackFinished", delayAttackTime);
        }

        private void ReloadBullets()
        {
            if (_autoFireActive)
            {
               // Debug.Log("<b>STOP ACTION</b> - START RELOADING");

                GameEvents.OnStopAction?.Invoke(this, EventArgs.Empty);
            }

            // Get amount of bullets to reload weapon
            if (_currentItemEquipped.CurrentBulletsAmount <= _currentItemEquipped.BulletsMagazine)
                _currentItemEquipped.BulletsToReload = _currentItemEquipped.CurrentBulletsAmount;
            else
                _currentItemEquipped.BulletsToReload = _currentItemEquipped.BulletsMagazine;

            _currentItemEquipped.RemainingReloadTime = _currentItemEquipped.ReloadTime;
            _currentItemEquipped.IsReloading = true;

            //Debug.Log($"<color=green>Start <b>RELOADING</b></color> - character: {_character.Data.UID}, bullets: {_currentItemEquipped.BulletsToReload}, reloading time: {_currentItemEquipped.ReloadTime}");

            OnStartReloading?.Invoke(new CustomEventArgs.PlayerAttackEventArgs(_currentItemEquipped, _character.Data.UID));
        }

        /// <summary>
        /// Rotates character towards the target from main field of view
        /// </summary>
        private void RotateTowardsNearestTarget()
        {
            _usingMainFieldOfView = true;
            
            var nearestTarget = _fov.NearestTarget;

            if (nearestTarget == null)
            {
                _usingMainFieldOfView = false;
                
                RotateTowardsSecondaryNearestTarget();
                
                return;
            }

            Debug.Log($"Rotate towards <b>Main</b> target: {nearestTarget.name}");
            
            Vector3 direction = nearestTarget.position - transform.position;
            direction.Normalize();
            Vector3 attackTargetDirection = new Vector3(direction.x, 0, direction.z).normalized;
            _character.Rotator.Rotate(attackTargetDirection);
        }
        
        /// <summary>
        /// Rotates character towards the target from secondary field of view
        /// </summary>
        private void RotateTowardsSecondaryNearestTarget()
        {
            var nearestTarget = _fovSecondary.NearestTarget;

            if (nearestTarget == null)
                return;

            Debug.Log($"Rotate towards <b>Secondary</b> target: {nearestTarget.name}");
            
            Vector3 direction = nearestTarget.position - transform.position;
            direction.Normalize();
            Vector3 attackTargetDirection = new Vector3(direction.x, 0, direction.z).normalized;
            _character.Rotator.Rotate(attackTargetDirection);
        }

        private void AttackFinished()
        {
            // If character start moving before attack was finished then skip all remaining attacks
            if (_character.State.CurrentState != Enums.CharacterStates.ATTACKING)
            {
                ResetAttackState();

                return;
            }

            ResetAttackState();
        }

        private void ResetAttackState()
        {
            _autoFireActive = false;

            _attackInProgress = false;

            _currentAmountAttacks = 0;

            _currentAttack = 0;

            _cacheTargets.Clear();

            _character.AttackFinished();
        }

        /*private void ComboAttack()
        {
            _character.State.SetState(Enums.CharacterStates.ATTACKING);

            _currentAttack++;

            Debug.Log($"<b>Attack</b> {_currentAttack} of {_currentAmountAttacks}");

            float delayAttackTime = _currentItemEquipped.AttackDelayTime + _currentItemEquipped.ComboDelayTime;

            Invoke("AttackFinished", delayAttackTime);

            StartCoroutine(ApplyDamageToCache());
        }*/

        private IEnumerator ApplyDamage()
        {
            float damage = _currentItemEquipped.Damage;

            // Get all visible targets by weapon configuration and apply damage to all of them in the attack area detection
            Transform[] targets = (_usingMainFieldOfView) ? _fov.VisibleTargets.ToArray() : _fovSecondary.VisibleTargets.ToArray();

            _cacheTargets.Clear();

            int targetsAmount = targets.Length;

            if (targetsAmount > 0)
            {
                yield return new WaitForSeconds(_currentItemEquipped.DelayToApplyDamage);

                for (int i = 0; i < targetsAmount; i++)
                {
                    IDamageable target = targets[i].GetComponent<IDamageable>();

                    if (target == null)
                        continue;

                    if (target.IsDead)
                        continue;

                    if (_canDebug)
                        Debug.Log($"Attack - Target: '{targets[i].name}', Health: {target.Health}, Damage: {damage}");

                    //target.TakeDamage(damage);
                    Debug.LogError($"PlayerAttackController::ApplyDamage", gameObject);

                    _cacheTargets.Add(target);
                }

                // Apply weapon usability consumption
                float durabilityPercUsed = _currentItemEquipped.MaxDurability * _currentItemEquipped.DurabilityAmountConsumptionByUse;
                _currentItemEquipped.CurrentDurability = Mathf.Clamp(_currentItemEquipped.CurrentDurability - durabilityPercUsed, 0, _currentItemEquipped.MaxDurability);
                OnMeleeAttack?.Invoke(new CustomEventArgs.PlayerAttackEventArgs(_currentItemEquipped, _character.Data.UID));
            }
        }

        private IEnumerator ApplyDamageToCache()
        {
            float damage = _currentItemEquipped.Damage;

            // Get all target visibles by weapon configuration and apply damage to all of them in the attack area detection
            IDamageable[] targets = _cacheTargets.ToArray();

            int targetsAmount = targets.Length;

            if (targetsAmount > 0)
            {
                yield return new WaitForSeconds(_currentItemEquipped.DelayToApplyDamage + _currentItemEquipped.ComboDelayTime);

                for (int i = 0; i < targetsAmount; i++)
                {
                    var target = targets[i];

                    if (target == null)
                        continue;

                    if (target.IsDead)
                        continue;

                    Debug.Log($"<color=red>Combo Attack</color> - Health: {target.Health}, Damage: {damage}");

                    //target.TakeDamage(damage);
                    Debug.LogError($"PlayerAttackController::ApplyDamage", gameObject);
                }

                // TODO: apply weapon usability based on type of weapon: melee (uses) or assault (ammo)

                // TODO: Check what happen if it is broken in the middle of combo, it should be stopped
            }
        }

        #endregion

        #region Public method

        public void ApplyMelee()
        {
            //_meleeTrailEffect?.Play();

            float damage = _currentItemEquipped.Damage;

            // Get all visible targets by weapon configuration and apply damage to all of them in the attack area detection
            Transform[] targets = (_usingMainFieldOfView) ? _fov.VisibleTargets.ToArray() : _fovSecondary.VisibleTargets.ToArray();

            _cacheTargets.Clear();

            int targetsAmount = targets.Length;

            if (targetsAmount > 0)
            {
                for (int i = 0; i < targetsAmount; i++)
                {
                    IDamageable target = targets[i].GetComponent<IDamageable>();

                    if (target == null)
                        continue;

                    if (target.IsDead)
                        continue;

                    if (_canDebug)
                        Debug.Log($"Attack - Target: '{targets[i].name}', Health: {target.Health}, Damage: {damage}");

                    //target.TakeDamage(damage);
                    Debug.LogError($"PlayerAttackController::ApplyDamage", gameObject);

                    _cacheTargets.Add(target);
                }

                // Apply weapon usability consumption
                float durabilityPercUsed = _currentItemEquipped.MaxDurability * _currentItemEquipped.DurabilityAmountConsumptionByUse;
                _currentItemEquipped.CurrentDurability = Mathf.Clamp(_currentItemEquipped.CurrentDurability - durabilityPercUsed, 0, _currentItemEquipped.MaxDurability);

                OnMeleeAttack?.Invoke(new CustomEventArgs.PlayerAttackEventArgs(_currentItemEquipped, _character.Data.UID));
            }
        }

        public void ApplyShooting()
        {
            OnShot?.Invoke(new CustomEventArgs.PlayerAttackEventArgs(_currentItemEquipped, _character.Data.UID));
        }

        #endregion
    }
}