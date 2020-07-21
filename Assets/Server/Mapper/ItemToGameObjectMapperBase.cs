using Assets.Server.Models;
using Assets.Server.Projection;
using UnityEngine;

namespace Assets.Server.Mapper
{
    public abstract class ItemToGameObjectMapperBase
    {
        protected GameObject Stage { get; private set; }
        protected StageCoordProjection StageCoordProjector { get; private set; }

        public ItemToGameObjectMapperBase(GameObject stage, StageCoordProjection stageCoordProjector)
        {
            Stage = stage;
            StageCoordProjector = stageCoordProjector;
        }

        public abstract GameObject GetGameObjectForItem(ItemModel item);
    }
}
