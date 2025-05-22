using UnityEngine;
using Mapbox.Unity.MeshGeneration.Modifiers;
using Mapbox.Unity.MeshGeneration.Data;

[CreateAssetMenu(menuName = "Mapbox/Modifiers/Building Tag Modifier")]
public class BuildingTagModifier : GameObjectModifier
{
    public override void Run(VectorEntity ve, UnityTile tile)
    {
        if(ve.GameObject != null)
        {
            // Set tag and layer
            ve.GameObject.tag = "Building";
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            ve.GameObject.layer = obstacleLayer;
        }
    }
}