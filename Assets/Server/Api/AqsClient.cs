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

        public AqsClient(string apiUrl, string token, string aqs, int page)
        {
            _apiUrl = apiUrl;
            _token = token;
            _aqs = aqs;
            _page = page;
        }

        public override IEnumerator Send()
        {
            var query = new Dictionary<string, string>
            {
                { "token", _token },
                { "page", _page.ToString() }
            };
            string json = $"{{ \"aqs\": {_aqs} }}";

            yield return Send("POST", _apiUrl, "api/aqs/query", query, json, true);
        }
    }
}
