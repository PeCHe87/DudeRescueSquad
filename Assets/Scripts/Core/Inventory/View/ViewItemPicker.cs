﻿using UnityEngine;

namespace DudeRescueSquad.Core.Inventory.View
{
    public class ViewItemPicker : MonoBehaviour
    {
        public event System.Action OnPicked;

        [SerializeField] private string _itemId = string.Empty;
        [SerializeField] private int _amount = 0;
        [SerializeField] private GameObject _art = null;

        private ItemPicker _picker = null;
        private bool _wasPicked = false;

        public string ItemId => _itemId;
        public bool WasPicked => _wasPicked;

        #region Public methods

        public void Setup(InventoryEntry inventory)
        {
            _picker = new ItemPicker(_itemId, _amount, inventory);
            Debug.Log($"Setup item picker: {name}");
        }

        #endregion

        #region Unity methods

        /// <summary>
        /// Triggered when something collides with the picker
        /// </summary>
        /// <param name="collider">Other.</param>
        public void OnTriggerEnter(Collider collider)
        {
            // if what's colliding with the picker ain't a characterBehavior, we do nothing and exit
            if (!collider.CompareTag("Player")) return;

            _picker.Pick(PickSuccess, PickFail);
        }

        #endregion

        #region Private methods

        private void PickSuccess()
        {
            _wasPicked = true;

            OnPicked?.Invoke();

            Debug.Log($"<color=green>Pick item:</color> {_itemId}");

            // TODO: show VFX

            // TODO: apply SFX

            // TODO: hide and destroy object
            _art.SetActive(false);

            Destroy(gameObject, 2);
        }

        private void PickFail(string error)
        {
            Debug.Log($"<color=red>Failed Pick item:</color> {_itemId}, error: {error}");

            // TODO: apply SFX

            // TODO: trigger some floating text explaining the problem
        }

        #endregion
    }
}