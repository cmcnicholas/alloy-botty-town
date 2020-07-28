namespace Assets.Server.Game
{
    public class JobModel : ItemModelBase
    {
        public string ParentAssetItemId { get; }

        public JobModel(string parentAssetItemId, string itemId, string designCode, string title, string subtitle, string signature) : base(itemId, designCode, title, subtitle, signature)
        {
            ParentAssetItemId = parentAssetItemId;
        }
    }
}
