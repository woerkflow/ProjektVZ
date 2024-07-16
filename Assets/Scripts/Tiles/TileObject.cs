using UnityEngine;

public class TileObject : MonoBehaviour {
    
    [Header("Tile Object")]
    public TileObjectBlueprint blueprint;
    
    private Tile _parentTile;

    public void SetParentTile(Tile parent) {
        _parentTile = parent;
        Debug.Log("SetParentTile(): _parentTile: " + _parentTile);
    }
    
    public void DestroyObject() {
        Debug.Log("DestroyObject(): _parentTile: " + _parentTile);
        Debug.Log("DestroyObject(): ruin: " + blueprint.ruin.GetComponent<TileObject>());
        
        _parentTile.ReplaceObject(
            blueprint.ruin.GetComponent<TileObject>()
        );
    }
}