using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class CharacterAnimations : MonoBehaviour
    {
        #region Private consts

        private const string _attackKey = "attack";
        private const string _runningKey = "running";

        #endregion

        #region Inspector properties

        [SerializeField] private Animator _anim = null;
        [SerializeField] private CharacterState _characterState = null;

        [Header("Animators based on equipment")]
        [SerializeField] private RuntimeAnimatorController _animNoEquipment = null;
        [SerializeField] private RuntimeAnimatorController _animMelee2Hands = null;
        [SerializeField] private RuntimeAnimatorController _animAssault2Hands = null;

        #endregion

        #region Private properties

        private PlayerData _data = null;
        private Enums.WeaponAttackType _currentWeaponAttackType = Enums.WeaponAttackType.NONE;
        private float _animationAttackSpeed = 1;

        #endregion

        #region Private methods

        private void Awake()
        {
            if (_characterState != null)
            _characterState.OnChanged += CharacterStateChanged;

            _anim.SetBool(_runningKey, false);

            Item.OnCollected += CollectItem;
        }

        private void OnDestroy()
        {
            if (_characterState != null)
                _characterState.OnChanged -= CharacterStateChanged;

            Item.OnCollected -= CollectItem;
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
            _anim.ResetTrigger(_attackKey);
        }

        private void Run()
        {
            _anim.speed = 1;
            _anim.SetBool(_runningKey, true);
            _anim.ResetTrigger(_attackKey);
        }

        private void Attack()
        {
            _anim.speed = _animationAttackSpeed;
            _anim.SetTrigger(_attackKey);
        }

        private void CollectItem(CustomEventArgs.CollectItemEventArgs args)
        {
            if (_data == null)
                return;

            if (!_data.UID.Equals(args.playerUID))
                return;

            // Check which kind of controller should be assigned
            var weaponItem = ((ItemWeaponData)args.item);
            var weaponType = weaponItem.AttackType;

            if (weaponType.Equals(_currentWeaponAttackType))
                return;

            _currentWeaponAttackType = weaponType;
            _animationAttackSpeed = weaponItem.AnimationSpeed;

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
            }
        }

        #endregion

        public void Init(PlayerData data)
        {
            _data = data;
        }
    }
}