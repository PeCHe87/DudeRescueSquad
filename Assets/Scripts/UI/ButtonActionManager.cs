using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DudeResqueSquad
{
    public class ButtonActionManager : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Character _playerCharacter = null;
        [SerializeField] private Image _imgFillEnableAction = null;
        [SerializeField] private Image _imgFillDurability = null;
        [SerializeField] private Image _imgBackgroundDurability = null;
        [SerializeField] private Image _imgCurrentItem = null;
        [SerializeField] private GameObject _content = null;
        [SerializeField] private TextMeshProUGUI _txtBulletsAmount = null;

        [Header("Reloading")]
        [SerializeField] private GameObject _progressBarReloading = null;
        [SerializeField] private Image _progressBarFillReloading = null;

        #endregion

        #region Private properties

        private PlayerAttackController _attackController = null;
        private PlayerData _data = null;
        private float _currentTime = 0;
        private float _refillTotalTime = 0;
        /// <summary>
        /// True if it is waiting to be usable again
        /// </summary>
        private bool _refilling = false;
        private Button _btn = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            _btn = GetComponent<Button>();

            _data = _playerCharacter.Data;

            _playerCharacter.OnEquipItem += EquipItem;

            _attackController = _playerCharacter.GetComponent<PlayerAttackController>();

            if (_attackController != null)
            {
                _attackController.OnShot += DoAction;
                _attackController.OnMeleeAttack += DoAction;
                _attackController.OnStartReloading += StartReloading;
                _attackController.OnFinishReloading += FinishReloading;
            }

            _imgFillEnableAction.fillAmount = 0;

            _content.SetActive(false);

            _progressBarReloading.SetActive(false);
        }

        private void Start()
        {
            if (_data == null || _data.CurrentWeaponEquipped == null)
            {
                HideDurability();
                HideBulletsAmount();
            }
        }

        private void OnDestroy()
        {
            _playerCharacter.OnEquipItem -= EquipItem;

            if (_attackController != null)
            {
                _attackController.OnShot -= DoAction;
                _attackController.OnMeleeAttack -= DoAction;
                _attackController.OnStartReloading -= StartReloading;
                _attackController.OnFinishReloading -= FinishReloading;
            }
        }

        private void Update()
        {
            if (_refilling)
            {
                _currentTime = Mathf.Clamp(_currentTime + Time.deltaTime, 0, _refillTotalTime);

                _imgFillEnableAction.fillAmount = 1 - (_currentTime / _refillTotalTime);

                if (_currentTime >= _refillTotalTime)
                {
                    _refilling = false;
                    _currentTime = 0;
                    _refillTotalTime = 0;
                    _btn.enabled = true;
                }
            }

            // Check reloading status
            if (_data.CurrentWeaponEquipped != null && _data.CurrentWeaponEquipped.IsReloading)
            {
                UpdateReloadingProgressBar();
            }
        }

        private void UpdateBulletsAmount()
        {
            _txtBulletsAmount.text = $"<size=40>{_data.CurrentWeaponEquipped.CurrentBulletsMagazine}</size>/<size=30>{_data.CurrentWeaponEquipped.CurrentBulletsAmount}</size>";
        }

        private void UpdateDurability()
        {
            float durabilityProgress = _data.CurrentWeaponEquipped.CurrentDurability / _data.CurrentWeaponEquipped.MaxDurability;

            _imgFillDurability.fillAmount = durabilityProgress;
        }

        private bool IsAssault()
        {
            if (_data.CurrentWeaponEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_1_HAND ||
                _data.CurrentWeaponEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_2_HANDS ||
                _data.CurrentWeaponEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_RIFLE)
                return true;

            return false;
        }

        private void EquipItem()
        {
            if (_data.CurrentWeaponEquipped == null)
                return;

            _imgCurrentItem.sprite = _data.CurrentWeaponEquipped.PreviewPic;

            if (!_content.activeSelf)
                _content.SetActive(true);

            // Check if it is an assault weapon to show bullets or not
            if (_data.CurrentWeaponEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_1_HAND ||
                _data.CurrentWeaponEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_2_HANDS ||
                _data.CurrentWeaponEquipped.AttackType == Enums.WeaponAttackType.ASSAULT_RIFLE)
            {
                ShowBulletsAmount();

                // Hide durability
                HideDurability();
            }
            else
            {
                HideBulletsAmount();

                // Show durability
                ShowDurability();
            }
        }

        private void StartRefillActionTime(float refillTime)
        {
            _refilling = true;
            _imgFillEnableAction.fillAmount = 1;
            _btn.enabled = false;
            _currentTime = 0;
            _refillTotalTime = refillTime;
        }

        private void ShowBulletsAmount()
        {
            UpdateBulletsAmount();

            _txtBulletsAmount.enabled = true;
        }

        private void HideBulletsAmount()
        {
            _txtBulletsAmount.enabled = false;
        }

        private void ShowDurability()
        {
            UpdateDurability();

            _imgFillDurability.enabled = true;
            _imgBackgroundDurability.enabled = true;
        }

        private void HideDurability()
        {
            _imgFillDurability.enabled = false;
            _imgBackgroundDurability.enabled = false;
        }

        private void UpdateReloadingProgressBar()
        {
            _progressBarFillReloading.fillAmount = _data.CurrentWeaponEquipped.RemainingReloadTime / _data.CurrentWeaponEquipped.ReloadTime;
        }

        #endregion

        #region Events

        private void DoAction(CustomEventArgs.PlayerAttackEventArgs e)
        {
            if (_refilling)
                return;

            if (_data.CurrentWeaponEquipped == null)
                return;

            if (!_data.CurrentWeaponEquipped.AutoFire)
                StartRefillActionTime(_data.CurrentWeaponEquipped.AttackDelayTime);
            //else
            //    Debug.Log($"UI - Update bullets {_data.CurrentWeaponEquipped.CurrentBulletsMagazine}");

            // If it is an assault weapon consume bullets, else check if it should show durability consumption
            if (IsAssault())
                UpdateBulletsAmount();
            else
                UpdateDurability();
        }

        private void FinishReloading(CustomEventArgs.PlayerAttackEventArgs e)
        {
            // Hide reloading progress bar
            _progressBarReloading.SetActive(false);

            // Update texts related with bullets
            UpdateBulletsAmount();

            // Enable text related with bullets
            _txtBulletsAmount.enabled = true;
        }

        private void StartReloading(CustomEventArgs.PlayerAttackEventArgs e)
        {
            // Disable text related with bullets
            _txtBulletsAmount.enabled = false;

            _progressBarFillReloading.fillAmount = 1;
            _progressBarReloading.SetActive(true);
        }

        #endregion

        #region Public methods

        public void ProcessAction()
        {
            GameEvents.OnProcessAction?.Invoke(this, System.EventArgs.Empty);
        }

        #endregion
    }
}