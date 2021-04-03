using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class NotifyTargetDetection : MonoBehaviour
    {
        [SerializeField] private FieldOfView _fieldOfView = default;

        private void Awake()
        {
            //_fieldOfView = GetComponent<FieldOfView>();

            if (_fieldOfView == null)
            {
                Debug.LogError("FieldOfView component is required");
                return;
            }
            
            _fieldOfView.OnDetectNewTarget += DetectNewTarget;
            _fieldOfView.OnStopDetecting += StopDetection;
        }

        private void OnDestroy()
        {
            if (_fieldOfView != null)
            {
                _fieldOfView.OnDetectNewTarget -= DetectNewTarget;
                _fieldOfView.OnStopDetecting -= StopDetection;
            }
        }

        private void StopDetection()
        {
            GameEvents.OnEnemyTargetChanged?.Invoke(this, new CustomEventArgs.EnemyTargetedArgs(null));
        }

        private void DetectNewTarget(Transform target)
        {
            GameEvents.OnEnemyTargetChanged?.Invoke(this, new CustomEventArgs.EnemyTargetedArgs(target));
        }
    }
}