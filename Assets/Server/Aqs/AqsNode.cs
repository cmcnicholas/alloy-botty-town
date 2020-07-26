using System.Collections.Generic;
using System.Linq;

namespace Assets.Server.Aqs
{
    public class AqsNode
    {
        public string Type { get; set; }
        public IList<AqsNode> Children { get; } = new List<AqsNode>();
        public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        public AqsNode(string type)
        {
            Type = type;
        }

        public string Stringify()
        {
            var properties = Properties.Select(kp => "\"" + kp.Key + "\": " + kp.Value);
            var children = Children.Select(c => c.Stringify());
            return "{\"type\": \"" + Type + "\", \"properties\": {" + string.Join(",", properties) + "}, \"children\": [" + string.Join(",", children) + "]}";
        }

        public void SetPropertyObject(string name, string value)
        {
            Properties[name] = string.IsNullOrEmpty(value) ? "null" : value;
        }

        public void SetPropertyString(string name, string value)
        {
            Properties[name] = string.IsNullOrEmpty(value) ? "null" : "\"" + value + "\"";
        }
    }
}
