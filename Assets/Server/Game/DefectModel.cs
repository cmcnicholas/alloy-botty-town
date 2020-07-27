namespace Assets.Server.Game
{
    public class DefectModel
    {
        public string ParentAssetItemId { get; }
        public string ItemId { get; }
        public string Signature { get; }

        public DefectModel(string parentAssetItemId, string itemId, string signature)
        {
            ParentAssetItemId = parentAssetItemId;
            ItemId = itemId;
            Signature = signature;
        }
    }
}
