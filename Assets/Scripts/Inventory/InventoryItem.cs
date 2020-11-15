using UnityEngine;
using UnityEditor;

namespace DudeResqueSquad.Inventory
{
    /// <summary>
    /// A ScriptableObject that represents any item that can be put in an inventory.
    /// </summary>
    /// <remarks>
    /// In practice, you are likely to use a subclass such as `ActionItem` or `EquipableItem`.
    /// </remarks>
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Public properties

        public GUIStyle _foldoutStyle = null;

        #endregion

        #region Private properties

        private string _itemID = string.Empty;
        private string _displayName = string.Empty;
        private Sprite _icon = null;
        private string _description = string.Empty;
        private bool _stackable = false;
        private bool _drawInventoryItem = true;

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

        public virtual void DrawCustomInspector()
        {
            _foldoutStyle = new GUIStyle(EditorStyles.foldout);
            _foldoutStyle.fontStyle = FontStyle.Bold;

            _drawInventoryItem = EditorGUILayout.Foldout(_drawInventoryItem, "InventoryItem Data", _foldoutStyle);

            if (!_drawInventoryItem)
                return;

            SetItemID(EditorGUILayout.TextField("ItemID (clear to reset", GetItemID()));
            SetDisplayName(EditorGUILayout.TextField("Display name", GetDisplayName()));
            SetDescription(EditorGUILayout.TextField("Description", GetDescription()));
            SetIcon((Sprite)EditorGUILayout.ObjectField("Icon", GetIcon(), typeof(Sprite), false));
            //SetPickup((Pickup)EditorGUILayout.ObjectField("Pickup", GetPickup(), typeof(Pickup), false));
            SetStackable(EditorGUILayout.Toggle("Stackable", IsStackable()));
        }

        #endregion

        #region Implement ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(_itemID))
            {
                _itemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }

        #endregion
    }
}