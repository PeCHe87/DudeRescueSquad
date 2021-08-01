using DudeRescueSquad.Core.Characters;
using DudeRescueSquad.Core.Inventory;
using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    public abstract class BaseWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] protected string _displayName = string.Empty;
        [Tooltip("Type of weapon: Assault, Melee, throwable, etc")]
        [SerializeField] protected Enums.ItemTypes _type = Enums.ItemTypes.NONE;
        [SerializeField] protected float _radiusView = 2;
        [SerializeField] protected float _angleView = 2;
        [Tools.Information("Indicates in which hand this item will be attached when equipped.", Tools.InformationAttribute.InformationType.Info, false)]
        public bool _leftHand = true;
        [Tooltip("an offset that will be applied to the weapon once attached to the center of the WeaponAttachment transform.")]
        public Vector3 _attachmentOffset = Vector3.zero;

        protected CharacterAbilityHandleWeapon _characterHandleWeapon = null;
        protected Character _characterOwner = null;

        #region IWeapon implementation

        public string DisplayName { get => _displayName; }
        public string WeaponID { get; set; }
        public Enums.ItemTypes WeaponType { get => _type; }
        public float AngleView { get => _angleView; }
        public float RadiusView { get => _radiusView; }
        public bool IsLeftHand { get => _leftHand; }
        public Vector3 WeaponAttachmentOffset { get => _attachmentOffset; }

        public abstract void Initialization(Character characterOwner);
        public abstract void WeaponInputStart();
        public abstract void TurnWeaponOff();

        #endregion
    }
}