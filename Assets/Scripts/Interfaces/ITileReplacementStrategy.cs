using UnityEngine;

public interface ITileReplacementStrategy {
    
    void ReplaceTileObject(Tile tile, GameObject newTileObject);
}
