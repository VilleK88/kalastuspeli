using UnityEngine;
using Mapbox.Unity.MeshGeneration.Modifiers;
using Mapbox.Unity.MeshGeneration.Data;
using Unity.VisualScripting.Antlr3.Runtime;

[CreateAssetMenu(menuName = "Mapbox/Modifiers/Building Tag Modifier")]
public class BuildingTagModifier : GameObjectModifier
{
    public override void Run(VectorEntity ve, UnityTile tile)
    {
        //base.Run(ve, tile);
        if(ve.GameObject != null)
        {
            ve.GameObject.tag = "Building";
            //int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            //ve.GameObject.layer = obstacleLayer;
        }
    }
}