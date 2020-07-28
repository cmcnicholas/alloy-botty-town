using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Assets.Server.ApiModels
{
    public class ItemCreateWebRequestModel
    {
        public string DesignCode;
        public string Collection;
        public IDictionary<string, string> Attributes = new Dictionary<string, string>();
        public IDictionary<string, IList<string>> Parents = new Dictionary<string, IList<string>>();

        public ItemCreateWebRequestModel(string designCode, string collection)
        {
            DesignCode = designCode;
            Collection = collection;
        }

        public void SetAttributeString(string attributeCode, string value)
        {
            Attributes.Add(attributeCode, "\"" + UnityWebRequest.EscapeURL(value) + "\"");
        }

        public void SetAttributeLink(string attributeCode, IList<string> value)
        {
            Attributes.Add(attributeCode, "[" + string.Join(",", "\"" + value + "\"") + "]");
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
            builder.Append("\"designCode\": \"" + DesignCode + "\",");
            builder.Append("\"collection\": \"" + Collection + "\",");
            builder.Append("\"attributes\": [" + string.Join(",", Attributes.Select(pair => "{\"attributeCode\": \"" + pair.Key + "\", \"value\": " + pair.Value + "}")) + "],");
            builder.Append("\"parents\": {" + string.Join(",", Parents.Select(pair => "\"" + pair.Key + "\": [" + string.Join(",", pair.Value.Select(v => "\"" + v + "\"")) + "]")) + "}");
            builder.Append("}");

            return builder.ToString();
        }
    }
}
