using UnityEngine;

public class EmptyStrategy : ITileReplacementStrategy {
    
    public void ReplaceTileObject(Tile tile, GameObject newTileObject) {
        tile.RotateObject(Tile.GetRandomRotation());
        GameObject tileObject = Object.Instantiate(newTileObject, tile.GetSpawnPoint().position, tile.objectRotation, tile.transform);
        tile.tileObject = tileObject.GetComponent<TileObject>();
    }
}