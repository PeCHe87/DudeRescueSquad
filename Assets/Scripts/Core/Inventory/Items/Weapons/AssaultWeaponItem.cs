using System.Threading.Tasks;
using UnityEngine;

namespace DudeRescueSquad.Core.Inventory.Items.Weapons
{
    [System.Serializable]
    public class AssaultWeaponItem : BaseItem
    {
        [UnityEngine.SerializeReference] private int _currentAmmo = 0;

        private bool _isReloading = false;

        public int CurrentAmmo => _currentAmmo;
        public bool IsReloading => _isReloading;

        public AssaultWeaponItem(string instanceId, string templateId, int ammo)
        {
            _instanceId = instanceId;
            _templateId = templateId;

            _type = Enums.ItemTypes.WEAPON_ASSAULT;

            _isReloading = false;

            UpdateCurrentAmmo(ammo);
        }

        public void UpdateCurrentAmmo(int amount)
        {
            _currentAmmo = amount;
        }

        public void ConsumeAmmo(int amount)
        {
            _currentAmmo = Mathf.Clamp(_currentAmmo - amount, 0, _currentAmmo);
        }

        public async void ReloadAmmo(float time, int amount, System.Action<string, int> callback)
        {
            _isReloading = true;

            var delay = Mathf.FloorToInt(time * 1000);

            await Task.Delay(delay);

            _isReloading = false;

            UpdateCurrentAmmo(amount);

            callback?.Invoke(_templateId, _currentAmmo);
        }
    }
}