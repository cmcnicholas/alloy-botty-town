namespace Assets.Server.Game
{
    public class TempDefectModel
    {
        public string ParentAssetItemId { get; }
        public string TempId { get; }

        public TempDefectModel(string parentAssetItemId, string tempId)
        {
            ParentAssetItemId = parentAssetItemId;
            TempId = tempId;
        }
    }
}
