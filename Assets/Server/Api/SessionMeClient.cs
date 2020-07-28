using Assets.Server.ApiModels;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Server.Api
{
    public class SessionMeClient : ApiClientBase<SessionMeWebResponseModel>
    {
        private string _apiUrl;
        private string _token;

        public SessionMeClient(string apiUrl, string token)
        {
            _apiUrl = apiUrl;
            _token = token;
        }

        public override IEnumerator Send()
        {
            var query = new Dictionary<string, string>
            {
                { "token", _token },
            };

            yield return Send("GET", _apiUrl, $"api/session/me", query, null, true);
        }
    }
}
