namespace Assets.Server.Models
{
    public class ItemModel
    {
        public string ItemId { get; }
        public float WorldX { get; }
        public float WorldZ { get; }

        public ItemModel(string itemId, float worldX, float worldZ)
        {
            ItemId = itemId;
            WorldX = worldX;
            WorldZ = worldZ;
        }
    }
}
