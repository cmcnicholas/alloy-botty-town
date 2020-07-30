namespace Assets.Server.Game
{
    public abstract class ItemModelBase
    {
        public string ItemId { get; set; }
        public string DesignCode { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Signature { get; set; }

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
