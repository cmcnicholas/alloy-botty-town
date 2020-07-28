using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Server.Game
{
    public class AssetModel : ItemModelBase
    {
        public IGeometryObject Geometry { get; }
        public GameObject GameObject { get; set; } // will be set later
        public IDictionary<string, JobModel> Jobs { get; } = new Dictionary<string, JobModel>();
        public IDictionary<string, InspectionModel> Inspections { get; } = new Dictionary<string, InspectionModel>();
        public IDictionary<string, DefectModel> Defects { get; } = new Dictionary<string, DefectModel>();
        private AssetController _assetController;

        public AssetModel(string itemId, string designCode, string title, string subtitle, string signature, IGeometryObject geometry) : base(itemId, designCode, title, subtitle, signature)
        {
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
