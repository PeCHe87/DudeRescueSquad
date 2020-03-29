using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class Character : MonoBehaviour
    {
        public class MovementEventArgs : EventArgs
        {
            public float x;
            public float y;

            public MovementEventArgs(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }

        #region Inspector properties

        [SerializeField] private PlayerData _data = null;
        [SerializeField] private float _speedMovement = 0;
        [SerializeField] private float _maxDistanceDelta = 2;
        [SerializeField] private RotateTowardsTarget _rotator = null;
        [SerializeField] private float _delayAttackTime = 1;
        [SerializeField] private bool _canDebug = false;

        [Header("Item equipped")]
        [SerializeField] private Transform _weaponPivot = null;

        #endregion

        #region Public properties

        public PlayerData Data { get => _data; }

        #endregion

        #region Private properties

        private ICharacterMovement _movement = null;
        private CharacterState _state = null;
        private Transform _characterTransform = null;
        private Vector3 _targetDirection = Vector3.zero;
        private bool _isMoving = false;
        private bool _isAttacking = false;
        private GameObject _currentItemEquipped = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            _characterTransform = transform;

            _movement = GetComponent<ICharacterMovement>();

            if (_movement != null)
            {
                _movement.OnDoAction += DoAction;
                _movement.OnStartMoving += StartMoving;
                _movement.OnStopMoving += StopMoving;
            }

            _state = GetComponent<CharacterState>();

            Item.OnCollect += CollectItem;
        }

        private void Start()
        {
            _state.SetState(CharacterState.CharacterStates.IDLE);
        }

        private void OnDestroy()
        {
            if (_movement != null)
            {
                _movement.OnDoAction -= DoAction;
                _movement.OnStartMoving -= StartMoving;
                _movement.OnStopMoving -= StopMoving;
            }

            Item.OnCollect -= CollectItem;
        }

        private void Update()
        {
            if (!_isMoving)
                return;

            if (_isAttacking)
                return;

            // Do movement
            Vector3 currentPosition = _characterTransform.position;
            //float x = _movement.Horizontal();
            //float y = _movement.Vertical();

            var dir = _movement.Direction();
            _targetDirection = new Vector3(dir.x, 0, dir.y).normalized; //new Vector3(x, 0, y).normalized;

            if (_canDebug)
            {
                Debug.DrawRay(currentPosition, _targetDirection * _speedMovement, Color.yellow);
                //Debug.Log(string.Format("[{0},{1}]", x, y));
            }

            _rotator.Rotate(_targetDirection);

            _characterTransform.position = Vector3.MoveTowards(_characterTransform.position, _targetDirection * _speedMovement, _maxDistanceDelta * Time.deltaTime);
        }

        private void StopMoving(object sender, EventArgs e)
        {
            if (!_isAttacking)
                _state.SetState(CharacterState.CharacterStates.IDLE);

            _isMoving = false;
        }

        private void StartMoving(object sender, MovementEventArgs e)
        {
            // Update state
            _state.SetState(CharacterState.CharacterStates.RUNNING);

            _isMoving = true;
        }

        private void DoAction(object sender, EventArgs e)
        {
            _state.SetState(CharacterState.CharacterStates.ATTACKING);

            _isAttacking = true;

            Invoke("AttackFinished", _delayAttackTime);
        }

        private void AttackFinished()
        {
            _isAttacking = false;

            if (_isMoving)
                _state.SetState(CharacterState.CharacterStates.RUNNING);
            else
                _state.SetState(CharacterState.CharacterStates.IDLE);
        }

        private void EquipItem(ItemData itemData)
        {
            if (itemData.Type == Enums.ItemType.WEAPON)
            {
                // Unequip current item if there was another one equipped
                if (_data.CurrentItem != null)
                    UnequipItem(_data.CurrentItem);

                // Set new current item equipped
                _data.CurrentItem = itemData;

                // Create new instance of the item equipped
                _currentItemEquipped = Instantiate(itemData.PrefabEquipable);

                // Positionate it at the right pivot
                _currentItemEquipped.transform.SetParent(_weaponPivot);
                _currentItemEquipped.transform.localPosition = Vector3.zero;
                _currentItemEquipped.transform.localRotation = Quaternion.identity;
            }
        }

        private void UnequipItem(ItemData currentItem)
        {
            Destroy(_currentItemEquipped);

            _data.CurrentItem = null;
        }

        #endregion

        #region Event methods

        private void CollectItem(ItemData item, string playerId)
        {
            if (_data == null)
                return;

            if (!_data.UID.Equals(playerId))
                return;

            EquipItem(item);
        }

        #endregion
    }
}