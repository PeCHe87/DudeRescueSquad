namespace DudeRescueSquad.Core.Inventory.Items.Weapons
{
    [System.Serializable]
    public class MeleeWeaponItem : BaseItem
    {
        public MeleeWeaponItem(string instanceId, string templateId)
        {
            _instanceId = instanceId;
            _templateId = templateId;

            _type = Enums.ItemTypes.WEAPON_MELEE;
        }
    }
}