using UnityEngine;
using DG.Tweening;

namespace DudeRescueSquad.UI
{
    public class PopupAnimator : MonoBehaviour
    {
        #region Singleton Instance

        private static PopupAnimator _instance;
        public static PopupAnimator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<PopupAnimator>();
                return _instance;
            }
        }

        #endregion

        #region Inspector properties

        [Header("Update value animation")]
        [SerializeField] private float _initialScale = 0.5f;
        [SerializeField] private float _animationDuration = 0.25f;
        [SerializeField] private Ease _ease = Ease.OutBack;

        #endregion

        #region Public methods

        public void AnimateScalePopup(Canvas canvas, Transform content)
        {
            content.localScale = Vector3.one * _initialScale;

            canvas.enabled = true;

            // Animate
            content.DOScale(Vector3.one, _animationDuration).SetEase(_ease);
        }

        #endregion
    }
}
