using GeoJSON.Net.Geometry;

namespace Assets.Server.Models
{
    public class ItemModel
    {
        public string ItemId { get; }
        public string DesignCode { get; }
        public IGeometryObject Geometry { get; set; }

        public ItemModel(string itemId, string designCode, IGeometryObject geometry)
        {
            ItemId = itemId;
            DesignCode = designCode;
            Geometry = geometry;
        }
    }
}
