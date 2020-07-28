using Assets.Server.ApiModels;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Server.Api
{
    public class JobUpdateClient : ApiClientBase<JobUpdateWebResponseModel>
    {
        private string _apiUrl;
        private string _token;
        private string _itemId;
        private ItemEditWebRequestModel _itemEdit;

        public JobUpdateClient(string apiUrl, string token, string itemId, ItemEditWebRequestModel itemEdit)
        {
            _apiUrl = apiUrl;
            _token = token;
            _itemEdit = itemEdit;
            _itemId = itemId;
        }

        public override IEnumerator Send()
        {
            var query = new Dictionary<string, string>
            {
                { "token", _token },
            };
            string json = $"{{ \"itemEditWebRequestModel\": {_itemEdit.Stringify()}, \"updateGeometryToMatchParent\": false }}";

            yield return Send("PUT", _apiUrl, $"api/job/" + _itemId, query, json, true);
        }
    }
}
