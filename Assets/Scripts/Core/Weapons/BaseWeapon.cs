using DudeRescueSquad.Core.Characters;
using DudeRescueSquad.Core.Inventory;
using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    public abstract class BaseWeapon : MonoBehaviour, IWeapon
    {
        [Header("Weapon Extra information")]
        [SerializeField] protected string _displayName = string.Empty;
        [Tooltip("Type of weapon: Assault, Melee, throwable, etc")]
        [SerializeField] protected Enums.ItemTypes _type = Enums.ItemTypes.NONE;
        [Tooltip("an offset that will be applied to the weapon once attached to the center of the WeaponAttachment transform.")]
        public Vector3 _attachmentOffset = Vector3.zero;

        protected CharacterAbilityHandleWeapon _characterHandleWeapon = null;
        protected Character _characterOwner = null;

        #region IWeapon implementation

        public abstract IWeaponDefinition WeaponData { get; }
        public string DisplayName { get => _displayName; }
        public string WeaponID { get; set; }
        public Enums.ItemTypes WeaponType { get => _type; }
        public Vector3 WeaponAttachmentOffset { get => _attachmentOffset; }

        public abstract void Initialization(Character characterOwner, BaseItem itemInstance);
        public abstract void WeaponInputStart();
        public abstract void TurnWeaponOff();
        public abstract bool CanBeUsed();
        public abstract void SetItemInstance(BaseItem itemInstance);

        #endregion
    }
}