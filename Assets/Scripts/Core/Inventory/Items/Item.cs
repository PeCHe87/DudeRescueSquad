namespace DudeRescueSquad.Core.Inventory
{
    public interface Item
    {
        string InstanceId { get; }
        string TemplateId { get; }
        Enums.ItemTypes Type { get; }
    }
}