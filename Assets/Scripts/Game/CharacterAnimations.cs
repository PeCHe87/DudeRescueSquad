using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class CharacterAnimations : MonoBehaviour
    {
        #region Private consts

        private const string _attackKey = "attack";
        private const string _runningKey = "running";
        private const string _autoFireKey = "autoFire";

        #endregion

        #region Inspector properties

        [SerializeField] private Animator _anim = null;
        [SerializeField] private CharacterState _characterState = null;

        [Header("Animators based on equipment")]
        [SerializeField] private RuntimeAnimatorController _animNoEquipment = null;
        [SerializeField] private RuntimeAnimatorController _animMelee2Hands = null;
        [SerializeField] private RuntimeAnimatorController _animAssault2Hands = null;
        [SerializeField] private RuntimeAnimatorController _animAssaultRifle = null;

        #endregion

        #region Private properties

        private PlayerData _data = null;
        private Enums.WeaponAttackType _currentWeaponAttackType = Enums.WeaponAttackType.NONE;
        private float _animationAttackSpeed = 1;
        private ItemWeaponData _currentWeapon = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            if (_characterState != null)
            _characterState.OnChanged += CharacterStateChanged;

            _anim.SetBool(_runningKey, false);

            GameEvents.OnCollectItem += CollectItem;
        }

        private void OnDestroy()
        {
            if (_characterState != null)
                _characterState.OnChanged -= CharacterStateChanged;

            GameEvents.OnCollectItem -= CollectItem;
        }

        private void CharacterStateChanged(Enums.CharacterStates state)
        {
            if (state == Enums.CharacterStates.IDLE)
                Idle();
            else if (state == Enums.CharacterStates.RUNNING)
                Run();
            else if (state == Enums.CharacterStates.ATTACKING)
                Attack();
        }

        private void Idle()
        {
            _anim.speed = 1;
            _anim.SetBool(_runningKey, false);
            _anim.SetBool(_autoFireKey, false);
            _anim.ResetTrigger(_attackKey);
        }

        private void Run()
        {
            _anim.speed = 1;
            _anim.SetBool(_runningKey, true);
            _anim.SetBool(_autoFireKey, false);
            _anim.ResetTrigger(_attackKey);
        }

        private void Attack()
        {
            _anim.speed = _animationAttackSpeed;
            _anim.SetTrigger(_attackKey);
            _anim.SetBool(_autoFireKey, (_currentWeapon != null) ? _currentWeapon.AutoFire : false);
        }

        private void CollectItem(object sender, CustomEventArgs.CollectItemEventArgs e)
        {
            if (_data == null)
                return;

            if (!_data.UID.Equals(e.playerUID))
                return;

            // Check which kind of controller should be assigned
            if (!(e.item is ItemWeaponData))
                return;

            _currentWeapon = ((ItemWeaponData)e.item);

            var weaponType = _currentWeapon.AttackType;

            if (weaponType.Equals(_currentWeaponAttackType))
                return;

            _currentWeaponAttackType = weaponType;
            _animationAttackSpeed = _currentWeapon.AnimationSpeed;

            // Update animator based on current equipment
            switch (weaponType)
            {
                case Enums.WeaponAttackType.NONE:
                    _anim.runtimeAnimatorController = _animNoEquipment;
                    break;

                case Enums.WeaponAttackType.MELEE_2_HANDS:
                    _anim.runtimeAnimatorController = _animMelee2Hands;
                    break;

                case Enums.WeaponAttackType.ASSAULT_2_HANDS:
                    _anim.runtimeAnimatorController = _animAssault2Hands;
                    break;

                case Enums.WeaponAttackType.ASSAULT_RIFLE:
                    _anim.runtimeAnimatorController = _animAssaultRifle;
                    break;
            }
        }

        #endregion

        public void Init(PlayerData data)
        {
            _data = data;
        }
    }
}