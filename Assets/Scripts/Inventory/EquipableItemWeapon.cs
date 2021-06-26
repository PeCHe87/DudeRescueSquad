using DudeResqueSquad.Stats;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DudeResqueSquad.Inventory
{
    [CreateAssetMenu(fileName = "EquipableWeapon", menuName = "Data/Inventory/Weapon", order = 0)]
    public class EquipableItemWeapon //: EquipableItem, IModifierProvider
    {/*
        #region Public properties

        public AnimatorOverrideController AnimatorOverride = null;
        public Combat.Weapon EquippedPrefab = null;
        public float WeaponDamage = 5f;
        public float PercentageBonus = 0;
        public float WeaponRange = 2f;
        public bool IsRightHanded = true;
        public Combat.Projectile Projectile = null;

        #endregion

        #region Private methods

#if UNITY_EDITOR
        private void SetWeaponRange(float newWeaponRange)
        {
            if (FloatEquals(WeaponRange, newWeaponRange))
                return;

            SetUndo("Set Weapon Range");
            WeaponRange = newWeaponRange;
            Dirty();
        }

        private void SetWeaponDamage(float newWeaponDamage)
        {
            if (FloatEquals(WeaponDamage, newWeaponDamage))
                return;

            SetUndo("Set Weapon Damage");
            WeaponDamage = newWeaponDamage;
            Dirty();
        }

        private void SetPercentageBonus(float newPercentageBonus)
        {
            if (FloatEquals(PercentageBonus, newPercentageBonus))
                return;

            SetUndo("Set Percentage Bonus");
            PercentageBonus = newPercentageBonus;
            Dirty();
        }

        private void SetIsRightHanded(bool newRightHanded)
        {
            if (IsRightHanded == newRightHanded)
                return;

            SetUndo(newRightHanded ? "Set as Right Handed" : "Set as Left Handed");
            IsRightHanded = newRightHanded;
            Dirty();
        }

        private void SetAnimatorOverride(AnimatorOverrideController newOverride)
        {
            if (newOverride == AnimatorOverride)
                return;

            SetUndo("Change AnimatorOverride");
            AnimatorOverride = newOverride;
            Dirty();
        }

        private void SetEquippedPrefab(Combat.Weapon newWeapon)
        {
            if (newWeapon == EquippedPrefab)
                return;

            SetUndo("Set Equipped Prefab");
            EquippedPrefab = newWeapon;
            Dirty();
        }

        private void SetProjectile(GameObject possibleProjectile)
        {
            if (!possibleProjectile.TryGetComponent<Combat.Projectile>(out Combat.Projectile newProjectile))
                return;

            if (newProjectile == Projectile)
                return;

            SetUndo("Set Projectile");
            Projectile = newProjectile;
            Dirty();
        }

        public bool FloatEquals(float value1, float value2)
        {
            return Mathf.Abs(value1 - value2) < .001f;
        }

#endif
        #endregion

        #region Public methods

#if UNITY_EDITOR
        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();
            SetEquippedPrefab((Combat.Weapon)EditorGUILayout.ObjectField("Equipped Prefab", EquippedPrefab, typeof(Object), false));
            SetWeaponDamage(EditorGUILayout.Slider("Weapon Damage", WeaponDamage, 0, 100));
            SetWeaponRange(EditorGUILayout.Slider("Weapon Range", WeaponRange, 1, 40));
            SetPercentageBonus(EditorGUILayout.IntSlider("Percentage Bonus", (int)PercentageBonus, -10, 100));
            SetIsRightHanded(EditorGUILayout.Toggle("Is Right Handed", IsRightHanded));
            SetAnimatorOverride((AnimatorOverrideController)EditorGUILayout.ObjectField("Animator Override", AnimatorOverride, typeof(AnimatorOverrideController), false));
            SetProjectile((GameObject)EditorGUILayout.ObjectField("Projectile", Projectile, typeof(Combat.Projectile), false));
        }
#endif

        #endregion

        #region Implements IModifierProvider

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return WeaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return PercentageBonus;
            }
        }

        #endregion
        */
    }
}