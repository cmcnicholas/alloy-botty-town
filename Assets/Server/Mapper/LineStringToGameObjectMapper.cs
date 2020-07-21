using System;
using Assets.Server.Models;
using Assets.Server.Projection;
using UnityEngine;

namespace Assets.Server.Mapper
{
    public class LineStringToGameObjectMapper : ItemToGameObjectMapperBase
    {
        public LineStringToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector) : base(stage, stageCoordProjector)
        {
        }

        public override GameObject GetGameObjectForItem(ItemModel item)
        {
            throw new NotImplementedException();
        }
    }
}
