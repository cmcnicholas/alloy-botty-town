using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Assets.Server.ApiModels
{
    public class ItemEditWebRequestModel
    {
        public string Collection;
        public string Signature;
        public IDictionary<string, string> Attributes = new Dictionary<string, string>();

        public ItemEditWebRequestModel(string collection, string signature)
        {
            Collection = collection;
            Signature = signature;
        }

        public void SetAttributes(JObject attributes)
        {
            foreach (var pair in attributes)
            {
                Attributes.Add(pair.Key, pair.Value.ToString(Formatting.None));
            }
        }

        public string Stringify()
        {
            var builder = new StringBuilder();

            builder.Append("{");
            builder.Append("\"signature\": \"" + Signature + "\",");
            builder.Append("\"collection\": \"" + Collection + "\",");
            builder.Append("\"attributes\": [" + string.Join(",", Attributes.Select(pair => "{\"attributeCode\": \"" + pair.Key + "\", \"value\": " + pair.Value + "}")) + "]");
            builder.Append("}");

            return builder.ToString();
        }
    }
}
