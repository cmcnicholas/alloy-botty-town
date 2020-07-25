namespace Assets.Server.Game
{
    public class JobModel
    {
        public string ParentAssetItemId { get; }
        public string ItemId { get; }
        public string Signature { get; }

        public JobModel(string parentAssetItemId, string itemId, string signature)
        {
            ParentAssetItemId = parentAssetItemId;
            ItemId = itemId;
            Signature = signature;
        }
    }
}
