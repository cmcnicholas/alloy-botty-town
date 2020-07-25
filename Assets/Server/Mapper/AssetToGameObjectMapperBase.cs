using Assets.Server.Game;
using Assets.Server.Projection;
using UnityEngine;

namespace Assets.Server.Mapper
{
    /// <summary>
    /// base implementation for the item to game object mapper,
    /// takes an item and makes a game object for it
    /// </summary>
    public abstract class AssetToGameObjectMapperBase
    {
        /// <summary>
        /// the stage to host the game objects on (the container)
        /// </summary>
        protected GameObject Stage { get; private set; }

        /// <summary>
        /// the coordinate projector for the stage, use to convert coordinates to game world
        /// </summary>
        protected StageCoordProjection StageCoordProjector { get; private set; }

        public AssetToGameObjectMapperBase(GameObject stage, StageCoordProjection stageCoordProjector)
        {
            Stage = stage;
            StageCoordProjector = stageCoordProjector;
        }

        /// <summary>
        /// creates a game object for an item
        /// </summary>
        public abstract GameObject CreateGameObjectForAsset(AssetModel asset);
    }
}
