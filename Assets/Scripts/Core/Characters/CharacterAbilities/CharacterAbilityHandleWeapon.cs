using UnityEngine;
using DudeRescueSquad.Tools;
using DudeRescueSquad.Core.Weapons;
using DudeResqueSquad;
using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.LevelManagement;

namespace DudeRescueSquad.Core.Characters
{
    /// <summary>
    /// Add this class to a character so it can use weapons
    /// Note that this component will trigger animations (if their parameter is present in the Animator), based on 
    /// the current weapon's Animations
    /// Animator parameters : defined from the Weapon's inspector
    /// </summary>
    [AddComponentMenu("DudeRescueSquad/Character/Abilities/Character Ability Handle Weapon")]
    public class CharacterAbilityHandleWeapon : CharacterAbility, IGameEventListener<GameLevelEvent>
    {
        #region Inspector properties

        /// This method is only used to display a helpbox text at the beginning of the ability's inspector
        public override string HelpBoxText()
        {
            return "This component will allow your character to pickup and use weapons. " +
                   "What the weapon will do is defined in the Weapon classes. " +
                   "This just describes the behaviour of the 'hand' holding the weapon, not the weapon itself. " +
                   "Here you can set an initial weapon for your character to start with, allow weapon pickup, " +
                   "and specify a weapon attachment (a transform inside of your character, could be just an empty child gameobject, or a subpart of your model. " +
                   "When the player wants to shoot then it checks the current input and if it corresponds it tells the weapon to start shooting.";
        }

        [Header("Weapon")]

        /// the initial weapon owned by the character
        [Tooltip("the initial weapon owned by the character")]
        public BaseWeapon InitialWeapon;
        /// if this is set to true, the character can pick up PickableWeapons
        [Tooltip("if this is set to true, the character can pick up PickableWeapons")]
        public bool CanPickupWeapons = true;
        [Header("Binding")]
        [Tooltip("the position the weapon will be attached to at Left hand. If left blank, will be this.transform.")]
        public Transform WeaponAttachmentLeftHand;
        [Tooltip("the position the weapon will be attached to at Right hand. If left blank, will be this.transform.")]
        public Transform WeaponAttachmentRightHand;
        /// the position from which projectiles will be spawned (can be safely left empty)
        [Tooltip("the position from which projectiles will be spawned (can be safely left empty)")]
        public Transform ProjectileSpawn;
        /// if this is true this animator will be automatically bound to the weapon
        [Tooltip("if this is true this animator will be automatically bound to the weapon")]
        public bool AutomaticallyBindAnimator = true;
        /// the ID of the AmmoDisplay this ability should update
        [Tooltip("the ID of the AmmoDisplay this ability should update")]
        public int AmmoDisplayID = 0;
        /// if this is true, IK will be automatically setup if possible
        [Tooltip("if this is true, IK will be automatically setup if possible")]
        public bool AutoIK = true;

        [Header("Input")]
        /// if this is true you won't have to release your fire button to auto reload
        [Tooltip("if this is true you won't have to release your fire button to auto reload")]
        public bool ContinuousPress = false;
        /// whether or not this character getting hit should interrupt its attack (will only work if the weapon is marked as interruptable)
        [Tooltip("whether or not this character getting hit should interrupt its attack (will only work if the weapon is marked as interruptable)")]
        public bool GettingHitInterruptsAttack = false;

        [Header("Buffering")]
        /// whether or not attack input should be buffered, letting you prepare an attack while another is being performed, making it easier to chain them
        [Tooltip("whether or not attack input should be buffered, letting you prepare an attack while another is being performed, making it easier to chain them")]
        public bool BufferInput;
        /// if this is true, every new input will prolong the buffer
        [Condition("BufferInput", true)]
        [Tooltip("if this is true, every new input will prolong the buffer")]
        public bool NewInputExtendsBuffer;
        /// the maximum duration for the buffer, in seconds
        [Condition("BufferInput", true)]
        [Tooltip("the maximum duration for the buffer, in seconds")]
        public float MaximumBufferDuration = 0.25f;
        /// if this is true, and if this character is using GridMovement, then input will only be triggered when on a perfect tile
        [Condition("BufferInput", true)]
        [Tooltip("if this is true, and if this character is using GridMovement, then input will only be triggered when on a perfect tile")]
        public bool RequiresPerfectTile = false;

        [Header("Debug")]
        [ReadOnly]
        [Tooltip("the weapon currently equipped by the Character")]
        public BaseWeapon CurrentWeapon;

        [Header("Weapon area detection")]
        [Tooltip("Used to detect which enemies are in weapon range when the attack is triggered")]
        [SerializeField] private WeaponAreaDetection _weaponAreaDetection = default;

        /// an animator to update when the weapon is used
        public Animator CharacterAnimator { get; set; }

        public bool IsEquipped { get => CurrentWeapon != null; }

        #endregion

        #region Protected properties

        protected float _fireTimer = 0f;
        protected float _secondaryHorizontalMovement;
        protected float _secondaryVerticalMovement;
        //protected WeaponAim _weaponAim;
        //protected ProjectileWeapon _projectileWeapon;
        //protected WeaponIK _weaponIK;
        protected Transform _leftHandTarget = null;
        protected Transform _rightHandTarget = null;
        protected float _bufferEndsAt = 0f;
        protected bool _buffering = false;
        protected const string _weaponEquippedAnimationParameterName = "WeaponEquipped";
        protected const string _weaponEquippedIDAnimationParameterName = "WeaponEquippedID";
        protected int _weaponEquippedAnimationParameter;
        protected int _weaponEquippedIDAnimationParameter;
        //protected List<WeaponModel> _weaponModels;

        #endregion

        #region Private properties

        private Character _character = null;
        private DudeResqueSquad.FieldOfView _fieldOfView = null;
        private bool _isTargetDetected = false;
        private bool _attackIsPressing = false;
        private bool _attackPressed = false;
        private bool _weaponEquipped = false;

        #endregion

        #region Public properties

        public bool HasDetectedTarget { get => _isTargetDetected; }
        public Transform CurrentTarget { get => _fieldOfView.NearestTarget; }
        public Transform[] VisibleTargets { get => _fieldOfView.VisibleTargets.ToArray(); }
        public WeaponAreaDetection WeaponAreaDetection  => _weaponAreaDetection;

        #endregion

        #region Unity methods

        private void Awake()
        {
            //ButtonActionManager.OnStartAction += StartAttacking;
            //ButtonActionManager.OnStopAction += StopAttacking;
        }

        /// <summary>
        /// On enable, we start listening for GameEvents. You may want to extend that to listen to other types of events.
        /// </summary>
        private void OnEnable()
        {
            this.EventStartListening<GameLevelEvent>();
        }

        /// <summary>
        /// On disable, we stop listening for GameEvents. You may want to extend that to stop listening to other types of events.
        /// </summary>
        protected void OnDisable()
        {
            this.EventStopListening<GameLevelEvent>();
        }

        private void OnDestroy()
        {
            //ButtonActionManager.OnStartAction -= StartAttacking;
            //ButtonActionManager.OnStopAction -= StopAttacking;

            if (_fieldOfView != null)
            {
                _fieldOfView.OnDetectNewTarget -= TargetDetected;
                _fieldOfView.OnStopDetecting -= NoTargetDetected;
            }
        }

        #endregion

        #region Private methods

        private void NoTargetDetected()
        {
            _isTargetDetected = false;
        }

        private void TargetDetected(Transform newTarget)
        {
            _isTargetDetected = true;
        }

        private void StopAttacking(CustomEventArgs.StopActionEventArgs evtArgs)
        {
            // Check action type
            if (evtArgs.Type != Enums.ActionType.ATTACK) return;

            _attackPressed = false;
            _attackIsPressing = false;

            // Update character state
            _character.StopAction(Enums.CharacterState.ATTACKING);
        }

        private void StartAttacking(CustomEventArgs.StartActionEventArgs evtArgs)
        {
            // Check action type
            if (evtArgs.Type != Enums.ActionType.ATTACK) return;

            _attackPressed = true;
        }

        private void StartAction()
        {
            if (!_weaponEquipped) return;

            if (CurrentWeapon.WeaponType == Inventory.Enums.ItemTypes.WEAPON_ASSAULT)
            {
                // Check if weapon can start shooting or shot on release
                if (CurrentWeapon.WeaponData.AttackOnRelease) return;

                _attackPressed = true;
            }
        }

        private void StopAction(bool deadZoneRelease)
        {
            // Weapon is not equipped
            if (!_weaponEquipped) return;

            // Skip attack trigger if the weapon attack should be trigger on release but the release was done on dead zone
            if (CurrentWeapon.WeaponData.AttackOnRelease && deadZoneRelease) return;

            if (CurrentWeapon.WeaponType == Inventory.Enums.ItemTypes.WEAPON_ASSAULT)
            {
                if (CurrentWeapon.WeaponData.AttackOnRelease)
                {
                    UseWeapon();
                }
                else
                {
                    _attackPressed = false;
                    _attackIsPressing = false;

                    // Update character state
                    _character.StopAction(Enums.CharacterState.ATTACKING);
                }
            }
            else if (CurrentWeapon.WeaponType == Inventory.Enums.ItemTypes.WEAPON_MELEE)
            {
                UseWeapon();
            }
        }

        /// <summary>
        /// Causes the character to start shooting
        /// </summary>
        private void UseWeapon()
        {
            // Check if it is available to be used
            if (!CurrentWeapon.CanBeUsed()) return;

            // Check character state
            if (_character.State != Enums.CharacterState.ATTACKING)
            {
                // Check if it is possible based on character state
                if (!_character.CanStartAction(Enums.ActionType.ATTACK)) return;

                // Update character state
                _character.UpdateState(Enums.CharacterState.ATTACKING);
            }

            CurrentWeapon.WeaponInputStart();
        }

        /// <summary>
        /// The purpose of this method it to make possible test mobile UI input with keyboard keys
        /// </summary>
        private void TestAttackInput()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //UseWeapon();
                //StartAttacking(new CustomEventArgs.StartActionEventArgs(Enums.ActionType.ATTACK));
                StartAction();
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                //StopAttacking(new CustomEventArgs.StopActionEventArgs(Enums.ActionType.ATTACK));
                StopAction(false);
            }
        }

        #endregion

        #region Protected methods

        protected virtual void InstantiateWeapon(BaseWeapon newWeapon, string weaponID, bool combo = false)
        {
            var position = (newWeapon.WeaponData.IsLeftHand) ? WeaponAttachmentLeftHand.transform.position + newWeapon.WeaponAttachmentOffset : WeaponAttachmentRightHand.transform.position + newWeapon.WeaponAttachmentOffset;
            var rotation = (newWeapon.WeaponData.IsLeftHand) ? WeaponAttachmentLeftHand.transform.rotation : WeaponAttachmentRightHand.transform.rotation;

            CurrentWeapon = Instantiate(newWeapon, position, rotation);

            CurrentWeapon.transform.parent = (newWeapon.WeaponData.IsLeftHand) ? WeaponAttachmentLeftHand : WeaponAttachmentRightHand;
            CurrentWeapon.transform.localPosition = newWeapon.WeaponAttachmentOffset;
            CurrentWeapon.WeaponID = weaponID;

            // Turn off the gun's emitters.
            CurrentWeapon.Initialization(_character);

            // Setup field of view to detect enemy targets based on current weapon stats
            _fieldOfView.Setup(CurrentWeapon.WeaponData.AngleView, CurrentWeapon.WeaponData.RadiusDetection);

            _weaponEquipped = true;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Grabs various components and inits stuff
        /// </summary>
        public virtual void Setup()
        {
            /*
            _character = this.gameObject.MMGetComponentNoAlloc<Character>();
            _characterGridMovement = this.gameObject.GetComponent<CharacterGridMovement>();
            _weaponModels = new List<WeaponModel>();
            foreach (WeaponModel model in this.gameObject.GetComponentsInChildren<WeaponModel>())
            {
                _weaponModels.Add(model);
            }
            CharacterAnimator = _animator;
            // filler if the WeaponAttachment has not been set
            if (WeaponAttachment == null)
            {
                WeaponAttachment = transform;
            }
            if ((_animator != null) && (AutoIK))
            {
                _weaponIK = _animator.GetComponent<WeaponIK>();
            }
            
            */

            _character = GetComponent<Character>();

            _fieldOfView = _character.GetComponentInChildren<DudeResqueSquad.FieldOfView>();
            _fieldOfView.OnDetectNewTarget += TargetDetected;
            _fieldOfView.OnStopDetecting += NoTargetDetected;

            // Set the initial weapon if not null
            if (InitialWeapon != null)
            {
                ChangeWeapon(InitialWeapon, InitialWeapon.DisplayName, false);
            }
        }

        /// <summary>
        /// Changes the character's current weapon to the one passed as a parameter
        /// </summary>
        /// <param name="newWeapon">The new weapon.</param>
        public virtual void ChangeWeapon(BaseWeapon newWeapon, string weaponID, bool combo = false)
        {
            // if the character already has a weapon, we make it stop shooting
            if (CurrentWeapon != null)
            {
                CurrentWeapon.TurnWeaponOff();

                if (!combo)
                {
                    ShootStop();
                    Destroy(CurrentWeapon.gameObject);
                }
            }

            if (newWeapon != null)
            {
                InstantiateWeapon(newWeapon, weaponID, combo);

                // TODO: communicates about the weapon change
            }
            else
            {
                CurrentWeapon = null;
            }
        }

        /// <summary>
        /// Causes the character to stop shooting
        /// </summary>
        public virtual void ShootStop()
        {
            /*
            // if the Shoot action is enabled in the permissions, we continue, if not we do nothing
            if (!AbilityPermitted || (CurrentWeapon == null))
            {
                return;
            }

            if (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponIdle)
            {
                return;
            }

            if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponReload)
                || (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStart)
                || (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStop))
            {
                return;
            }

            if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBeforeUse) && (!CurrentWeapon.DelayBeforeUseReleaseInterruption))
            {
                return;
            }

            if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses) && (!CurrentWeapon.TimeBetweenUsesReleaseInterruption))
            {
                return;
            }

            if (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponUse)
            {
                return;
            }

            ForceStop();*/
        }

        #endregion

        #region ICharacterAbility implementation

        /// <summary>
        /// Gets input and triggers methods based on what's been pressed
        /// </summary>
        protected override void HandleInput()
        {
            if (CurrentWeapon == null) return;

            // For testing purposes
            TestAttackInput();

            // Check first attack by pressing attack button
            if (_attackPressed)
            {
                _attackPressed = false;
                _attackIsPressing = true;
                UseWeapon();
            }

            // Check assault weapon autofire
            if (_attackIsPressing)
            {
                if (IsAssaultWeapon(CurrentWeapon.WeaponData))
                {
                    var assaultWeaponData = CurrentWeapon.WeaponData as WeaponAssaultData;

                    if (assaultWeaponData.IsAutoFire)
                    {
                        UseWeapon();
                    }
                }
            }
        }

        private bool IsAssaultWeapon(IWeaponDefinition data)
        {
            return data.Type == WeaponType.ASSAULT_ONE_HAND ||
                data.Type == WeaponType.ASSAULT_TWO_HANDS;
        }

        public override void Initialization()
        {
            Setup();

            base.Initialization();
        }

        /// <summary>
        /// Every frame we check if it's needed to update the ammo display
        /// </summary>
        public override void Process()
        {
            /*
            HandleFeedbacks();
            UpdateAmmoDisplay();
            HandleBuffer();
            */
        }

        #endregion

        #region GameEventListener<GameLevelEvent> implementation

        /// <summary>
        /// Check different events related with game level
        /// </summary>
        /// <param name="eventData">Inventory event.</param>
        public virtual void OnGameEvent(GameLevelEvent eventData)
        {
            switch (eventData.EventType)
            {
                case GameLevelEventType.StartPlayerAction:
                    StartAction();
                    break;

                case GameLevelEventType.StopPlayerAction:
                    StopAction(eventData.DeadZoneRelease);
                    break;
            }
        }

        #endregion
    }
}