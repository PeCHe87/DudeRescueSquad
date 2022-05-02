namespace DudeRescueSquad.Core.Inventory
{
    [System.Serializable]
    public class BaseItem : Item
    {
        public string InstanceId { get => _instanceId; }
        public string TemplateId { get => _templateId; }
        public Enums.ItemTypes Type { get => _type; }

        [UnityEngine.SerializeReference] protected string _instanceId = string.Empty;
        [UnityEngine.SerializeReference] protected string _templateId = string.Empty;
        [UnityEngine.SerializeReference] protected Enums.ItemTypes _type = Enums.ItemTypes.NONE;
    }
}