using System;
using System.Collections.Generic;

namespace Assets.Server.Game
{
    public class GameStore
    {
        private IDictionary<string, AssetModel> _assets = new Dictionary<string, AssetModel>();
        private IDictionary<string, JobModel> _jobs = new Dictionary<string, JobModel>();
        private IDictionary<string, InspectionModel> _inspections = new Dictionary<string, InspectionModel>();

        public AssetModel GetAsset(string itemId)
        {
            return _assets.ContainsKey(itemId) ? _assets[itemId] : null;
        }

        public JobModel GetJob(string itemId)
        {
            return _jobs.ContainsKey(itemId) ? _jobs[itemId] : null;
        }

        public InspectionModel GetInspection(string itemId)
        {
            return _inspections.ContainsKey(itemId) ? _inspections[itemId] : null;
        }

        public void AddAsset(AssetModel model)
        {
            // remove existing asset if we have it already
            if (_assets.ContainsKey(model.ItemId))
            {
                RemoveAsset(model.ItemId);
            }

            // add asset to dictionary
            _assets.Add(model.ItemId, model);
        }

        public void AddJob(JobModel model)
        {
            // check asset exists
            if (!_assets.ContainsKey(model.ParentAssetItemId))
            {
                throw new Exception("parent asset item id not present, cannot add job: " + model.ItemId);
            }
            var asset = _assets[model.ParentAssetItemId];

            // remove existing job if we already have it
            if (_jobs.ContainsKey(model.ItemId))
            {
                RemoveJob(model.ItemId);
            }

            // add the job to global dictionary and assets dictionary
            _jobs.Add(model.ItemId, model);
            asset.Jobs.Add(model.ItemId, model);

            // set fire to the asset as we have at least 1 job
            asset.GetAssetController().SetFire(true);
        }

        public void AddInspection(InspectionModel model)
        {
            // check asset exists
            if (!_assets.ContainsKey(model.ParentAssetItemId))
            {
                throw new Exception("parent asset item id not present, cannot add inspection: " + model.ItemId);
            }
            var asset = _assets[model.ParentAssetItemId];
            
            // remove existing inspection if we already have it
            if (_inspections.ContainsKey(model.ItemId))
            {
                RemoveInspection(model.ItemId);
            }

            // add the inspection to global dictionary and assets dictionary
            _inspections.Add(model.ItemId, model);
            asset.Inspections.Add(model.ItemId, model);

            // set animation to the asset as we have at least 1 inspection
            asset.GetAssetController().SetInspect(true);
        }

        public void RemoveAsset(string itemId)
        {
            // check we have an asset to remove
            if (!_assets.ContainsKey(itemId))
            {
                return;
            }

            // get the asset
            var asset = _assets[itemId];

            // clear all jobs on the asset
            foreach (var job in asset.Jobs.Values)
            {
                RemoveJob(job.ItemId);
            }
            
            // clear all inspections on the asset
            foreach (var inspection in asset.Inspections.Values)
            {
                RemoveInspection(inspection.ItemId);
            }

            // dispose of the asset game object
            UnityEngine.Object.Destroy(asset.GameObject);

            // remove the asset from the dictionary
            _assets.Remove(itemId);
        }

        public void RemoveJob(string itemId)
        {
            // check we have a job to remove
            if (!_jobs.ContainsKey(itemId))
            {
                return;
            }
            var job = _jobs[itemId];

            // check we have a parent asset
            if (!_assets.ContainsKey(job.ParentAssetItemId))
            {
                throw new Exception("job has no parent asset, cannot remove: " + itemId);
            }

            // get the parent asset and remove the job
            var asset = _assets[job.ParentAssetItemId];
            asset.Jobs.Remove(itemId);

            // if the asset has no more jobs, remove fire
            if (asset.Jobs.Count == 0)
            {
                asset.GetAssetController().SetFire(false);
            }

            // remove the job from the dictionary
            _jobs.Remove(itemId);
        }

        public void RemoveInspection(string itemId)
        {
            // check we have an inspection to remove
            if (!_inspections.ContainsKey(itemId))
            {
                return;
            }
            var inspection = _inspections[itemId];

            // check we have a parent asset
            if (!_assets.ContainsKey(inspection.ParentAssetItemId))
            {
                throw new Exception("inspection has no parent asset, cannot remove: " + itemId);
            }

            // get the parent asset and remove the inspection
            var asset = _assets[inspection.ParentAssetItemId];
            asset.Inspections.Remove(itemId);

            // if the asset has no more inspections, remove animation
            if (asset.Inspections.Count == 0)
            {
                asset.GetAssetController().SetInspect(false);
            }

            // remove the inspection from the dictionary
            _inspections.Remove(itemId);
        }
    }
}
