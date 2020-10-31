using UnityEngine;
using UnityEditor;

namespace DudeResqueSquad
{
    public class InventoryItem : MonoBehaviour
    {
        #region Private properties

        private string _itemID = string.Empty;
        private string _displayName = string.Empty;
        private Sprite _icon = null;
        private string _description = string.Empty;
        private bool _stackable = false;

        #endregion

        #region Public methods

        public string GetItemID()
        {
            return _itemID;
        }

        public string GetDisplayName()
        {
            return _displayName;
        }

        public string GetDescription()
        {
            return _description;
        }

        public Sprite GetIcon()
        {
            return _icon;
        }

        public bool IsStackable()
        {
            return _stackable;
        }

        public void SetUndo(string message)
        {
            Undo.RecordObject(this, message);
        }

        public void Dirty()
        {
            EditorUtility.SetDirty(this);
        }

        public void SetItemID(string newItemID)
        {
            if (_itemID == newItemID)
                return;

            SetUndo("Change ItemID");

            _itemID = newItemID;

            Dirty();
        }


        public void SetDisplayName(string newDisplayName)
        {
            if (newDisplayName == _displayName)
                return;

            SetUndo("Change Display Name");

            _displayName = newDisplayName;

            Dirty();
        }

        public void SetDescription(string newDescription)
        {
            if (newDescription == _description)
                return;

            SetUndo("Change Description");

            _description = newDescription;

            Dirty();
        }

        public void SetIcon(Sprite newIcon)
        {
            if (_icon == newIcon)
                return;

            SetUndo("Change Icon");

            _icon = newIcon;

            Dirty();
        }

        /*public void SetPickup(Pickup newPickup)
        {
            if (pickup == newPickup) return;
            SetUndo("Change Pickup");
            pickup = newPickup;
            Dirty();
        }*/

        public void SetStackable(bool newStackable)
        {
            if (_stackable == newStackable)
                return;

            SetUndo(_stackable ? "Set Not Stackable" : "Set Stackable");

            _stackable = newStackable;

            Dirty();
        }

        #endregion
    }
}