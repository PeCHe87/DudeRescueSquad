using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DudeResqueSquad
{
    public class WeaponHUD : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private TextMeshProUGUI _txtWeaponEquipped = null;
        [SerializeField] private Image _imgIcon = null;

        #endregion

        #region Private properties

        private Canvas _canvas = null;

        #endregion

        #region Private methods

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;

            _txtWeaponEquipped.text = string.Empty;
            _imgIcon.sprite = null;

            GameManager.Instance.OnPlayerCollectWeapon += WeaponCollected;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnPlayerCollectWeapon -= WeaponCollected;
        }

        private void WeaponCollected(ItemWeaponData weaponData, PlayerData playerData)
        {
            _txtWeaponEquipped.text = weaponData.DisplayName;
            _imgIcon.sprite = weaponData.PreviewPic;

            _canvas.enabled = true;
        }

        #endregion
    }
}