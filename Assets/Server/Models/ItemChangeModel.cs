namespace Assets.Server.Models
{
    public class ItemChangeModel
    {
        public ItemModel Item { get; }
        public ItemChangeType Type { get; }

        public ItemChangeModel(ItemModel item, ItemChangeType type)
        {
            Item = item;
            Type = type;
        }
    }
}
