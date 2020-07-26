using Assets.Server.ApiModels;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Server.Api
{
    public class AqsClient : ApiClientBase<AqsResponseWebModel>
    {
        private string _apiUrl;
        private string _token;
        private string _aqs;
        private int _page;
        private int _pageSize;
        private bool _isJoin;

        public AqsClient(string apiUrl, string token, string aqs, bool isJoin, int page, int pageSize)
        {
            _apiUrl = apiUrl;
            _token = token;
            _aqs = aqs;
            _page = page;
            _pageSize = pageSize;
            _isJoin = isJoin;
        }

        public override IEnumerator Send()
        {
            var query = new Dictionary<string, string>
            {
                { "token", _token },
                { "page", _page.ToString() },
                { "pageSize", _pageSize.ToString() }
            };
            string json = $"{{ \"aqs\": {_aqs} }}";

            yield return Send("POST", _apiUrl, $"api/aqs/{(_isJoin ? "join" : "query")}", query, json, true);
        }
    }
}
