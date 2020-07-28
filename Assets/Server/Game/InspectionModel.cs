namespace Assets.Server.Game
{
    public class InspectionModel : ItemModelBase
    {
        public string ParentAssetItemId { get; }

        public InspectionModel(string parentAssetItemId, string itemId, string designCode, string title, string subtitle, string signature) : base(itemId, designCode, title, subtitle, signature)
        {
            ParentAssetItemId = parentAssetItemId;
        }
    }
}
