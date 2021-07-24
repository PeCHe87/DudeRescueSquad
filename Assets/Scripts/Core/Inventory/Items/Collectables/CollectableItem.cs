using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DudeRescueSquad.Core.Inventory.Items
{
    public class CollectableItem : Item
    {
        public Enums.ItemTypes Type { get => _type; }

        protected Enums.ItemTypes _type = Enums.ItemTypes.NONE;
    }
}