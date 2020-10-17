using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class Entity : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private EntityData _data = null;

        #endregion

        #region Private properties

        private EntityFollower _follower = null;
        private FieldOfView _fod = null;
        private float _health = 0;

        #endregion

        #region Public Properties

        public EntityData Data { get => _data; }
        public EntityFollower Follower { get => _follower; }

        #endregion

        #region Private methods

        private void Awake()
        {
            // Field of View events subscription
            _fod = GetComponent<FieldOfView>();

            if (_fod != null)
            {
                _fod.OnDetectNewTarget += DetectNewTarget;
                _fod.OnStopDetecting += StopDetection;
            }

            // TODO: FSM events subscription
        }

        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            if (_fod != null)
            {
                _fod.OnDetectNewTarget -= DetectNewTarget;
                _fod.OnStopDetecting -= StopDetection;
            }
        }

        private void DetectNewTarget(Transform target)
        {
            _follower.Target = target;
        }

        private void StopDetection()
        {
            _follower.Target = null;
        }

        #endregion

        #region Public Method

        public void Init()
        {
            _health = Mathf.Clamp(_data.CurrentHealth, 0, _data.MaxHealth);

            if (_fod != null)
            {
                _fod.Radius = _data.RadiusDetection;
                _fod.ViewAngle = _data.AngleDetection;
                _fod.TargetMask = _data.TargetMaskDetection;
                _fod.ObstacleMask = _data.ObstacleMaskDetection;
            }
        }

        public void InitMovement(EntityFollower followerTemplate)
        {
            _follower = Instantiate(followerTemplate);
            _follower.name = $"{this.name}_follower";
            _follower.transform.SetParent(this.transform.parent);
            _follower.transform.position = transform.position;
            _follower.Config(_data);

            var visualBehavior = GetComponent<EntityVisual>();

            if (visualBehavior != null)
                visualBehavior.Target = _follower.transform;
        }

        public void SetTarget(Transform target)
        {
            _follower.Target = target;
        }

        #endregion
    }
}