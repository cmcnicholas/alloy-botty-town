using System.Collections.Generic;

namespace Assets.Server.ApiModels
{
    public class AqsResponseWebModel
    {
        public IList<ItemWebModel> Results;
        public int TotalPages;
    }
}
