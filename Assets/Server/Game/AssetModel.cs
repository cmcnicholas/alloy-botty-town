using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Server.Game
{
    public class AssetModel
    {
        public string ItemId { get; }
        public string DesignCode { get; }
        public IGeometryObject Geometry { get; }
        public GameObject GameObject { get; set; }
        public IDictionary<string, JobModel> Jobs { get; } = new Dictionary<string, JobModel>();
        public IDictionary<string, InspectionModel> Inspections { get; } = new Dictionary<string, InspectionModel>();
        private AssetController _assetController;

        public AssetModel(string itemId, string designCode, IGeometryObject geometry)
        {
            ItemId = itemId;
            DesignCode = designCode;
            Geometry = geometry;
        }

        public AssetController GetAssetController()
        {
            if (_assetController == null)
            {
                _assetController = GameObject.GetComponent<AssetController>();
            }
            return _assetController;
        }
    }
}
