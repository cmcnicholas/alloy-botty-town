namespace Assets.Server.Game
{
    public class DefectModel : ItemModelBase
    {
        public string ParentAssetItemId { get; }

        public DefectModel(string parentAssetItemId, string itemId, string designCode, string title, string subtitle, string signature) : base(itemId, designCode, title, subtitle, signature)
        {
            ParentAssetItemId = parentAssetItemId;
        }
    }
}
