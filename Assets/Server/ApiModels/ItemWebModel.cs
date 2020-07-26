using System.Collections.Generic;

namespace Assets.Server.ApiModels
{
    public class ItemWebModel
    {
        public string ItemId;
        public string DesignCode;
        public IList<ItemAttributeWebModel> Attributes;
        public string Signature;
    }
}
