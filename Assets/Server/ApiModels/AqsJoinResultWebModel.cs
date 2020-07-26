using System.Collections.Generic;

namespace Assets.Server.ApiModels
{
    public class AqsJoinResultWebModel
    {
        public string ItemId;
        public IList<AqsJoinResultJoinQueryWebModel> JoinQueries;
    }
}
