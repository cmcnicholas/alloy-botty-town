using System.Collections.Generic;

namespace Assets.Server.ApiModels
{
    public class AqsResponseWebModel
    {
        public IList<ItemWebModel> Results;
        public IList<AqsJoinResultWebModel> JoinResults;
        public int PageSize;
    }
}
