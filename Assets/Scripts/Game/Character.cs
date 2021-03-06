﻿using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DudeResqueSquad
{
    public class Character : MonoBehaviour
    {
        public Action OnEquipItem;

        #region Inspector properties

        [SerializeField] private PlayerData _data = null;
        [SerializeField] private float _maxSpeedMovement = 0;
        [SerializeField] private float _maxDistanceDelta = 2;
        [SerializeField] private RotateTowardsTarget _rotator = null;
        [SerializeField] private float _delayAttackTime = 1;
        [SerializeField] private bool _moveTransformMode = false;
        [SerializeField] private ForceMode _forceMode = ForceMode.Acceleration;
        [SerializeField] private Vector3 _velocity = Vector3.zero;
        [SerializeField] private float _velocityStep = 0;
        [SerializeField] private float _initialSpeedVelocity = 0;
        [SerializeField] private float _angleThreshold = 30;
        [SerializeField] private bool _canDebug = false;
        [SerializeField] private bool _canBreak = false;
        [SerializeField] private int _maxFixedUpdatesPerFrame = 5;
        [SerializeField] private bool _maxSpeedConstant = true;
        [SerializeField] private float _rollingSpeedMovement = 0;

        [Header("Item equipped")]
        [SerializeField] private Transform _weaponPivot = null;
        [SerializeField] private Transform _leftHandPivot = null;

        [SerializeField] private GameObject _cube;

        #endregion

        #region Public properties

        public PlayerData Data { get => _data; }
        public CharacterState State { get => _state; set => _state = value; }
        public RotateTowardsTarget Rotator { get => _rotator; }

        #endregion

        #region Private properties

        private ICharacterMovement _movement = null;
        private CharacterState _state = null;
        private Transform _characterTransform = null;
        private Vector3 _targetDirection = Vector3.zero;
        private bool _isMoving = false;
        private GameObject _currentItemEquipped = null;
        private Rigidbody _rb = null;
        private float _currentSpeed = 0;
        private Vector3 _oldDirection = Vector3.zero;
        private float _angle = 0;
        private int _fixedUpdateFrames = 0;
        private bool _isRolling = false;
        private Vector3 _rollingDirection = Vector3.zero;
        private Vector3 _lastTargetDirection = Vector3.zero;

        #endregion

        #region Private methods

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            _characterTransform = transform;

            _movement = GetComponent<ICharacterMovement>();

            if (_movement != null)
            {
                _movement.OnStartMoving += StartMoving;
                _movement.OnStopMoving += StopMoving;
                _movement.OnDoAction += DoAction;
                _movement.OnStartAction += StartAction;
                //_movement.OnStopAction += StopAction;
            }

            GameEvents.OnStopAction += StopAction;

            _state = GetComponent<CharacterState>();

            GameEvents.OnCollectItem += CollectItem;

            GameEvents.OnStartPlayerRolling += StartRolling;
            GameEvents.OnStopPlayerRollingAnimation += StopRolling;
            GameEvents.OnStartPlayerRollingAnimation += StartRollingAnimation;
        }

        private void Start()
        {
            _state.SetState(Enums.CharacterStates.IDLE);

            if (_data != null)
            {
                _data.Clean();

                var characterAnimations = GetComponent<CharacterAnimations>();

                if (characterAnimations != null)
                    characterAnimations.Init(_data);
            }
        }

        private void OnDestroy()
        {

            if (_movement != null)
            {
                _movement.OnStartMoving -= StartMoving;
                _movement.OnStopMoving -= StopMoving;
                _movement.OnDoAction -= DoAction;
                _movement.OnStartAction -= StartAction;
                //_movement.OnStopAction -= StopAction;
            }

            GameEvents.OnStopAction -= StopAction;

            GameEvents.OnCollectItem -= CollectItem;
            
            GameEvents.OnStartPlayerRolling -= StartRolling;
            GameEvents.OnStopPlayerRollingAnimation -= StopRolling;
            GameEvents.OnStartPlayerRollingAnimation -= StartRollingAnimation;
        }

        private void Update()
        {
            Debug.DrawRay(transform.position, _lastTargetDirection * _rollingSpeedMovement, Color.cyan);
            
            if (!_isMoving)
                return;

            if (_state.CurrentState == Enums.CharacterStates.ATTACKING)
                return;

            // Do movement if not rolling
            if (!_isRolling)
            {
                var dir = _movement.Direction();
                _targetDirection = new Vector3(dir.x, 0, dir.y).normalized;

                if (_canDebug)
                    Debug.DrawRay(_characterTransform.position, _targetDirection * _maxSpeedMovement, Color.yellow);
                
                //if (_state.CurrentState == Enums.CharacterStates.RUNNING)

                if (_rotator.Rotate(_targetDirection))
                {
                    _lastTargetDirection = _targetDirection;
                }
            }
            else
            {
                if (_canDebug)
                    Debug.DrawRay(_characterTransform.position, _targetDirection * _rollingSpeedMovement, Color.yellow);
            }
            
        }

        private void LateUpdate()
        {
            if (!_isRolling)
            {
                if (_state.CurrentState == Enums.CharacterStates.RUNNING)
                    _rotator.Rotate(_targetDirection);
            }

            _fixedUpdateFrames = 0;
            _oldDirection = _targetDirection;
        }

        private void FixedUpdate()
        {
            _fixedUpdateFrames++;

            //Debug.Log($"Fixed update frames: {_fixedUpdateFrames}");

            if (_fixedUpdateFrames >= _maxFixedUpdatesPerFrame)
                return;

            _velocity = _rb.velocity;

            if (_isRolling)
            {
                PerformRolling();

                return;
            }

            if (!_isMoving)
                return;

            if (_state.CurrentState == Enums.CharacterStates.ATTACKING)
                return;

            if (_moveTransformMode)
                return;
            
            // Move by running
            var dir = _movement.Direction();
            _targetDirection = new Vector3(dir.x, 0, dir.y).normalized;

            var position = transform.position;
            
            // TODO: check what happen if these two lines aren't present, because direction is calculated at Update (may be it isn't needed calculate it for each fixed frame)
            Debug.DrawRay(position, _targetDirection, Color.green);
            Debug.DrawRay(position, _oldDirection, Color.red);
            
            // TODO: check if it is neeeded avoid movement when direction.magnitude is less than a threshold value
            MoveWithChangeVelocityForce();
        }

        private void PerformRolling()
        {
            /*_rb.velocity = Vector3.zero;

            _rb.AddForce(_rollingDirection * _rollingSpeedMovement, ForceMode.VelocityChange);
            
            //Debug.Log("Perform rolling ---- ");
            
            Debug.DrawRay(_characterTransform.position, _rollingDirection * _rollingSpeedMovement, Color.magenta);*/
        }

        private void MoveTowards()
        {
            _characterTransform.position = Vector3.MoveTowards(_characterTransform.position, _targetDirection * _maxSpeedMovement, _maxDistanceDelta * Time.deltaTime);
        }

        private void MoveWithForce()
        {
            UpdatePhysicsMovement();            
        }

        private void MoveWithChangeVelocityForce()
        {
            if (_maxSpeedConstant)
            {
                _rb.velocity = Vector3.zero;

                _rb.AddForce(_targetDirection * _maxSpeedMovement, ForceMode.VelocityChange);
            }
            else
            {
                if (_oldDirection != Vector3.zero)
                {
                    _angle = Vector3.Angle(_targetDirection, _oldDirection);

                    if (Mathf.Abs(_angle) > _angleThreshold)
                    {
                        _currentSpeed = _initialSpeedVelocity;
                        Debug.Log("Reset current speed");
                    }
                }

                // Current speed increases gradually until reaches max speed allowed
                _currentSpeed = Mathf.Clamp(_currentSpeed + (_velocityStep * Time.fixedDeltaTime), 0, _maxSpeedMovement);

                _rb.AddForce(_targetDirection * _currentSpeed, ForceMode.VelocityChange);

                CancelMomentum();
            }
        }

        private void CancelMomentum()
        {
            _rb.AddForce(Vector3.right * -_rb.velocity.x, ForceMode.VelocityChange);
            _rb.AddForce(Vector3.forward * -_rb.velocity.z, ForceMode.VelocityChange);
        }

        private void UpdatePhysicsMovement()
        {
            // Reset velocity at each fixed frame to avoid slippering or skid behaviors on character movement
            _rb.velocity = Vector3.zero;

            // Current speed increases gradually until reaches max speed allowed
            _currentSpeed = Mathf.Clamp(_currentSpeed + (_velocityStep * Time.fixedDeltaTime), 0, _maxSpeedMovement);

            _rb.AddForce(_targetDirection * _currentSpeed, _forceMode);
        }

        private void StopForceMovement()
        {
            _rb.velocity = Vector3.zero;
            _currentSpeed = 0;
        }

        private void StopMoving(object sender, EventArgs e)
        {
            if (_state.CurrentState != Enums.CharacterStates.ATTACKING)
                _state.SetState(Enums.CharacterStates.IDLE);

            _isMoving = false;

            if (!_moveTransformMode)
                StopForceMovement();

            _targetDirection = Vector3.zero;
            _oldDirection = Vector3.zero;
            _fixedUpdateFrames = 0;
        }

        private void DoAction(object sender, CustomEventArgs.TouchEventArgs e)
        {
            /*if (_data == null)
                return;

            if (_data.CurrentWeaponEquipped == null)
                return;

            if (_data.CurrentWeaponEquipped.Type != Enums.ItemType.WEAPON)
                return;

            var weaponItem = _data.CurrentWeaponEquipped;

            if (weaponItem.AttackType == Enums.WeaponAttackType.ASSAULT_1_HAND ||
                weaponItem.AttackType == Enums.WeaponAttackType.ASSAULT_2_HANDS ||
                weaponItem.AttackType == Enums.WeaponAttackType.ASSAULT_RIFLE)
            {
                // Rotates character towards touched direction
                Vector3 currentPosition = _characterTransform.position;
                var touchPosition = e.touchPosition;

                _cube.transform.position = touchPosition;
            }*/
        }

        private void StartMoving(object sender, CustomEventArgs.MovementEventArgs e)
        {
            // Update state
            _state.SetState(Enums.CharacterStates.RUNNING);

            _isMoving = true;

            _currentSpeed = _initialSpeedVelocity;
            
            _oldDirection = Vector3.zero;
        }

        private void EquipItem(ItemData itemData)
        {
            if (itemData.Type == Enums.ItemType.WEAPON)
            {
                // Unequip current item if there was another one equipped
                if (_data.CurrentWeaponEquipped != null)
                    UnequipItem(_data.CurrentWeaponEquipped);
                
                // Set new current item equipped
                _data.CurrentWeaponEquipped = (ItemWeaponData)itemData;
                
                /*                
                // Create new instance of the item equipped
                _currentItemEquipped = Instantiate(itemData.PrefabEquipable);

                // Positionate it at the right pivot
                var localPos = _currentItemEquipped.transform.localPosition;
                var localRot = _currentItemEquipped.transform.localRotation;
                _currentItemEquipped.transform.SetParent(_weaponPivot);
                _currentItemEquipped.transform.localPosition = localPos;
                _currentItemEquipped.transform.localRotation = localRot;

                // Check if it is a two hands assault weapon
                if (_data.CurrentWeaponEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_2_HANDS || _data.CurrentWeaponEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_RIFLE)
                {
                    var twoHandsFollower = _currentItemEquipped.GetComponent<TwoHandsWeaponFollower>();

                    if (twoHandsFollower != null)
                        twoHandsFollower.Init(_leftHandPivot);
                }

                if (GameManager.Instance != null)
                    GameManager.Instance.OnPlayerCollectWeapon?.Invoke(_data.CurrentWeaponEquipped, _data);*/
                
                itemData.AssetReference.InstantiateAsync().Completed += OnItemInstantiationCompleted;
            }
            else if (itemData.Type == Enums.ItemType.KEY)
            {
                ItemKeyData keyData = (ItemKeyData)itemData;

                if (keyData.KeyType == Enums.KeyType.REGULAR)
                    _data.RegularKeys++;
                else if (keyData.KeyType == Enums.KeyType.SPECIAL)
                    _data.SpecialKeys++;
                else if (keyData.KeyType == Enums.KeyType.SKELETON)
                    _data.SkeletonKeys++;

                Debug.Log($"Player: '{_data.UID}' collects a <b><color=green>key</color></b> of type: '{keyData.KeyType}'");

                GameManager.Instance.OnPlayerCollectKey?.Invoke(keyData.KeyType, _data);
            }
        }

        private void UnequipItem(ItemData currentItem)
        {
            Destroy(_currentItemEquipped);

            _data.CurrentWeaponEquipped = null;
        }

        private void OnItemInstantiationCompleted(AsyncOperationHandle<GameObject> obj)
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                return;
            }

            // Get the new instance of the item equipped
            _currentItemEquipped = obj.Result;

            // Position it at the right pivot
            var localPos = _currentItemEquipped.transform.localPosition;
            var localRot = _currentItemEquipped.transform.localRotation;
            _currentItemEquipped.transform.SetParent(_weaponPivot);
            _currentItemEquipped.transform.localPosition = localPos;
            _currentItemEquipped.transform.localRotation = localRot;
            
            // Check if it is a two hands assault weapon
            if (_data.CurrentWeaponEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_2_HANDS || _data.CurrentWeaponEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_RIFLE)
            {
                var twoHandsFollower = _currentItemEquipped.GetComponent<TwoHandsWeaponFollower>();

                if (twoHandsFollower != null)
                    twoHandsFollower.Init(_leftHandPivot);
            }

            if (GameManager.Instance != null)
                GameManager.Instance.OnPlayerCollectWeapon?.Invoke(_data.CurrentWeaponEquipped, _data);
        }
        
        private void StartAction(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void StopAction(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Event methods

        private void CollectItem(object sender, CustomEventArgs.CollectItemEventArgs e)
        {
            if (_data == null)
                return;

            var playerId = e.playerUID;

            if (!_data.UID.Equals(playerId))
                return;

            var item = e.item;

            EquipItem(item);

            OnEquipItem?.Invoke();
        }

        private void StartRolling(object sender, EventArgs e)
        {
            if (!_isRolling)
            {
                var dir = _lastTargetDirection;

                _rollingDirection = new Vector3(dir.x, 0, dir.y).normalized;

                _isRolling = true;
                
                // Update state
                _state.SetState(Enums.CharacterStates.ROLLING);

                Debug.Log("<color=green><b>START</b> ROLLLING</color>");
                
                Debug.DrawRay(transform.position, _lastTargetDirection * (_rollingSpeedMovement + 1), Color.magenta, 1);
            }
        }

        private void StartRollingAnimation(object sender, EventArgs e)
        {
            _rb.velocity = Vector3.zero;
            _rb.AddForce(_lastTargetDirection * _rollingSpeedMovement, ForceMode.Impulse);
        }

        private void StopRolling(object sender, EventArgs e)
        {
            if (_isRolling)
            {
                _rollingDirection = Vector3.zero;
                _isRolling = false;
                
                _rb.velocity = Vector3.zero;
                
                Debug.Log("<color=red><b>STOP</b> ROLLLING</color>");
                
                // Update state
                var nextState = (_movement.Direction() == Vector3.zero)
                    ? Enums.CharacterStates.IDLE
                    : Enums.CharacterStates.RUNNING;
                
                _state.SetState(nextState);
            }
        }
        
        #endregion

        #region Public methods

        public void AttackFinished()
        {
            if (_isMoving)
                _state.SetState(Enums.CharacterStates.RUNNING);
            else
                _state.SetState(Enums.CharacterStates.IDLE);
        }

        public Weapon GetWeapon()
        {
            return (_currentItemEquipped != null) ? _currentItemEquipped.GetComponent<Weapon>() : null;
        }

        #endregion
    }
}