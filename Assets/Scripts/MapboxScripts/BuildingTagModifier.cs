using UnityEngine;
using Mapbox.Unity.MeshGeneration.Modifiers;
using Mapbox.Unity.MeshGeneration.Data;

[CreateAssetMenu(menuName = "Mapbox/Modifiers/Building Tag Modifier")]
public class BuildingTagModifier : GameObjectModifier
{
    public override void Run(VectorEntity ve, UnityTile tile)
    {
        //base.Run(ve, tile);
        if(ve.GameObject != null)
        {
            ve.GameObject.tag = "Building";
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            ve.GameObject.layer = obstacleLayer;

            if(ve.GameObject.transform.localScale.y < 2f)
            {
                var scale = ve.GameObject.transform.localScale;
                scale.y = 2f;
                ve.GameObject.transform.localScale = scale;
            }
        }
    }
}