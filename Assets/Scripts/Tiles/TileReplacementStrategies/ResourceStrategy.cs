using UnityEngine;

public class ResourceStrategy : ITileReplacementStrategy {
    
    public void ReplaceTileObject(Tile tile, GameObject newTileObject) {
        TileObject newResource;
        
        if (newTileObject.name.Contains("RandomResource")) {
            tile.RotateObject(Tile.GetRandomRotation());
            newResource = tile.GetRandomResource();
        } else {
            newResource = newTileObject.GetComponent<TileObject>();
        }
        tile.tileObject = Object.Instantiate(newResource, tile.GetSpawnPoint().position, tile.GetRotation(), tile.transform);
    }
}
