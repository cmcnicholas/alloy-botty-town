using System;
using Assets.Server.Models;
using Assets.Server.Projection;
using GeoJSON.Net.Geometry;
using UnityEngine;

namespace Assets.Server.Mapper
{
    /// <summary>
    /// implementation to create game objects for linestring items
    /// </summary>
    public class LineStringToGameObjectMapper : ItemToGameObjectMapperBase
    {
        private Material _roadMaterial;

        public LineStringToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, Material roadMaterial) : base(stage, stageCoordProjector)
        {
            _roadMaterial = roadMaterial;
        }

        public override GameObject CreateGameObjectForItem(ItemModel item)
        {
            // get the geometry and expect it to be a linestring
            var itemLineString = item.Geometry as LineString;
            if (itemLineString == null)
            {
                throw new Exception("Expected item model geometry to be of type LineString");
            }

            // check we have enough coordinates
            int coordinateCount = itemLineString.Coordinates.Count;
            if (coordinateCount <= 1)
            {
                return null;
            }

            // get the vertices of the linestring in the game world
            var vertices = new Vector3[coordinateCount];
            for (int i = 0; i < coordinateCount; i++)
            {
                // calculate the world coordinates (metres) for the point in the linestring
                var coordinate = itemLineString.Coordinates[i];
                var coordinateMetres = WebMercatorProjection.LatLonToMeters(coordinate.Latitude, coordinate.Longitude);

                // project the metres to the stage
                var itemStageCoords = StageCoordProjector.MetresToStageCoordinate(coordinateMetres);

                // outside of map?
                if (itemStageCoords == null)
                {
                    return null;
                }

                vertices[i] = new Vector3(itemStageCoords[0], 0.01f /* off the floor */, itemStageCoords[1]);
            }

            // make a new game object, we will draw programatically
            var go = new GameObject();

            // add the renderer to the game object
            var renderer = go.AddComponent<LineRenderer>();
            renderer.material = _roadMaterial;
            renderer.startWidth = 10.0f;
            renderer.endWidth = 10.0f;

            // weird, we have to set the position count, then the positions...
            renderer.positionCount = coordinateCount;
            renderer.SetPositions(vertices);

            // make it lay down flat (else lines point upwards towards the Z)
            renderer.alignment = LineAlignment.TransformZ;
            renderer.transform.Rotate(90.0f, 90.0f, 90.0f, Space.Self);

            return go;
        }
    }
}
