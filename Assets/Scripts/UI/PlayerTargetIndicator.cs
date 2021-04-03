using UnityEngine;

namespace DudeResqueSquad
{
    public class PlayerTargetIndicator : MonoBehaviour
    {
        private string _entityId = default;
        private Transform _originalParent;
        private Canvas _canvas = null;
        private Transform _transform = default;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;

            _transform = transform;
            
            GameEvents.OnEnemyTargetChanged += TargetChanged;
        }

        private void OnDestroy()
        {
            GameEvents.OnEnemyTargetChanged -= TargetChanged;
        }

        private void TargetChanged(object sender, CustomEventArgs.EnemyTargetedArgs e)
        {
            if (e.target == null)
            {
                _canvas.enabled = false;
                _transform.SetParent(_originalParent);
                _entityId = string.Empty;
            }
            else
            {
                Entity entity = e.target.GetComponent<Entity>();

                if (entity == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(_entityId) && _entityId.Equals(entity.UID))
                {
                    return;
                }

                _entityId = entity.UID;
                
                _transform.SetParent(e.target);

                _transform.localPosition = new Vector3(0, 0.01f, 0);
                _transform.localRotation = Quaternion.Euler(90, 0, 0);
                _transform.localScale = Vector3.one * 0.001f;
                
                if (!_canvas.enabled)
                {
                    _canvas.enabled = true;
                }
            }
        }
    }
}