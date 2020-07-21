using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Assets.Server.ApiModels
{
    public class ItemAttributeWebModel
    {
        public string AttributeCode;
        public JToken Value;

        public string ValueAsString()
        {
            if (Value == null)
            {
                return null;
            }
            if (Value.Type == JTokenType.Null)
            {
                return null;
            }
            if (Value.Type != JTokenType.String)
            {
                throw new Exception("failed to get value as string of item attribute " + AttributeCode);
            }
            return Value.Value<string>();
        }

        public IGeometryObject ValueAsGeoJson()
        {
            if (Value == null)
            {
                return null;
            }
            if (Value.Type == JTokenType.Null)
            {
                return null;
            }
            if (Value.Type != JTokenType.Object)
            {
                throw new Exception("failed to get value as geojson of item attribute " + AttributeCode);
            }

            string type = Value.Value<string>("type");
            switch (type)
            {
                case "Point":
                    return JsonConvert.DeserializeObject<Point>(Value.ToString());
                case "MultiPoint":
                    return JsonConvert.DeserializeObject<MultiPoint>(Value.ToString());
                case "LineString":
                    return JsonConvert.DeserializeObject<LineString>(Value.ToString());
                case "MultiLineString":
                    return JsonConvert.DeserializeObject<MultiLineString>(Value.ToString());
                case "Polygon":
                    return JsonConvert.DeserializeObject<Polygon>(Value.ToString());
                case "MultiPolygon":
                    return JsonConvert.DeserializeObject<MultiPolygon>(Value.ToString());
                case "GeometryCollection":
                    return JsonConvert.DeserializeObject<GeometryCollection>(Value.ToString());
                default:
                    throw new Exception($"failed to get value as geojson, unhandled type '${type}' of item attribute " + AttributeCode);
            }
        }
    }
}
