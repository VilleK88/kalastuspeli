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
        }
    }
}