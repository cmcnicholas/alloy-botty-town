namespace Assets.Server.Models
{
    public class ItemModel
    {
        public string ItemId { get; }
        public string DesignCode { get; }
        public float WorldX { get; }
        public float WorldZ { get; }

        public ItemModel(string itemId, string designCode, float worldX, float worldZ)
        {
            ItemId = itemId;
            DesignCode = designCode;
            WorldX = worldX;
            WorldZ = worldZ;
        }
    }
}
