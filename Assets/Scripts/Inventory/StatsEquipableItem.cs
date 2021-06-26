using System.Collections.Generic;
using DudeResqueSquad.Stats;
using UnityEngine;

namespace DudeResqueSquad.Inventory
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Equipable Item"))]
    public class StatsEquipableItem //: EquipableItem, IModifierProvider
    {
        /*
        [SerializeField] private List<Modifier> additiveModifiers = new List<Modifier>();
        [SerializeField] private List<Modifier> percentageModifiers = new List<Modifier>();

        [System.Serializable]
        struct Modifier
        {
            public Stat stat;
            public float value;
        }   

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var modifier in additiveModifiers)
            {
                if (modifier.stat == stat)
                {
                    yield return modifier.value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var modifier in percentageModifiers)
            {
                if (modifier.stat == stat)
                {
                    yield return modifier.value;
                }
            }
        }
        
#if UNITY_EDITOR
        
        void AddModifier(List<Modifier> modifierList)
        {
            SetUndo("Add Modifier");
            modifierList?.Add(new Modifier());
            Dirty();
        }

        void RemoveModifier(List<Modifier>modifierList, int index)
        {
            SetUndo("Remove Modifier");
            modifierList?.RemoveAt(index);
            Dirty();
        }

        void SetStat(List<Modifier> modifierList, int i, Stat stat)
        {
            if (modifierList[i].stat == stat) return;
            SetUndo("Change Modifier Stat");
            Modifier mod = modifierList[i];
            mod.stat = stat;
            modifierList[i] = mod;
            Dirty();
        }
        
#endif
*/
    }
}