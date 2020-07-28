namespace Assets.Server.Game
{
    public abstract class ItemModelBase
    {
        public string ItemId { get; }
        public string DesignCode { get; }
        public string Title { get; }
        public string Subtitle { get; }
        public string Signature { get; }

        public ItemModelBase(string itemId, string designCode, string title, string subtitle, string signature)
        {
            ItemId = itemId;
            DesignCode = designCode;
            Title = title;
            Subtitle = subtitle;
            Signature = signature;
        }
    }
}
