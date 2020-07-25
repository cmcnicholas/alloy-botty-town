namespace Assets.Server.Game
{
    public class InspectionModel
    {
        public string ParentAssetItemId { get; }
        public string ItemId { get; }
        public string Signature { get; }

        public InspectionModel(string parentAssetItemId, string itemId, string signature)
        {
            ParentAssetItemId = parentAssetItemId;
            ItemId = itemId;
            Signature = signature;
        }
    }
}
