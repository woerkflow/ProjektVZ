using UnityEngine;

public class TileObject : MonoBehaviour {
    
    [Header("Tile Object")]
    public TileObjectBlueprint blueprint;
    
    public Tile parentTile { get; set; }
    
    public void DestroyObject() {
        parentTile.ReplaceObject(
            blueprint.ruin.GetComponent<TileObject>()
        );
    }
}