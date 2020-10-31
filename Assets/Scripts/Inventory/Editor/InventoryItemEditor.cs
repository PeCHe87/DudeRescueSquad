using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace DudeResqueSquad
{
    public class InventoryItemEditor : EditorWindow
    {
        #region Private properties

        private InventoryItem _selected = null;

        #endregion

        #region Private methods

        private void OnSelectionChange()
        {
            var item = EditorUtility.InstanceIDToObject(Selection.activeInstanceID) as InventoryItem;

            if (item == null)
            {
                return;
            }

            _selected = item;

            Repaint();
        }

        private void OnGUI()
        {
            if (!_selected)
            {
                EditorGUILayout.HelpBox("No Dialogue Selected", MessageType.Error);
                return;
            }

            EditorGUILayout.HelpBox($"{_selected.name}/{_selected.GetDisplayName()}", MessageType.Info);

            _selected.SetItemID(EditorGUILayout.TextField("ItemID (clear to reset", _selected.GetItemID()));
            _selected.SetDisplayName(EditorGUILayout.TextField("Display name", _selected.GetDisplayName()));
            _selected.SetDescription(EditorGUILayout.TextField("Description", _selected.GetDescription()));
            _selected.SetIcon((Sprite)EditorGUILayout.ObjectField("Icon", _selected.GetIcon(), typeof(Sprite), false));
            //_selected.SetPickup((Pickup)EditorGUILayout.ObjectField("Pickup", selected.GetPickup(), typeof(Pickup), false));
            _selected.SetStackable(EditorGUILayout.Toggle("Stackable", _selected.IsStackable()));
        }

        #endregion

        #region Public methods

        [MenuItem("Window/InventoryItem Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(InventoryItemEditor), false, "InventoryItem");
        }

        public static void ShowEditorWindow(InventoryItem item)
        {
            InventoryItemEditor window = GetWindow(typeof(InventoryItemEditor), false, "InventoryItem") as InventoryItemEditor;

            if (item)
            {
                window.OnSelectionChange();
            }
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            InventoryItem item = EditorUtility.InstanceIDToObject(instanceID) as InventoryItem;

            if (item != null)
            {
                ShowEditorWindow(item);
                return true;
            }

            return false;
        }

        #endregion
    }
}