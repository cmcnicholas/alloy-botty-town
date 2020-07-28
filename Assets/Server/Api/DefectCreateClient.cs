using Assets.Server.ApiModels;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Server.Api
{
    public class DefectCreateClient : ApiClientBase<DefectCreateWebResponseModel>
    {
        private string _apiUrl;
        private string _token;
        private ItemCreateWebRequestModel _itemCreate;

        public DefectCreateClient(string apiUrl, string token, ItemCreateWebRequestModel itemCreate)
        {
            _apiUrl = apiUrl;
            _token = token;
            _itemCreate = itemCreate;
        }

        public override IEnumerator Send()
        {
            var query = new Dictionary<string, string>
            {
                { "token", _token },
            };
            string json = $"{{ \"itemCreateWebRequestModel\": {_itemCreate.Stringify()} }}";

            yield return Send("POST", _apiUrl, $"api/defect", query, json, true);
        }
    }
}
