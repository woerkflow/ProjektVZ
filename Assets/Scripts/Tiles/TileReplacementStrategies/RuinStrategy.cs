using UnityEngine;

public class RuinStrategy : ITileReplacementStrategy {
    
    public void ReplaceTileObject(Tile tile, GameObject newTileObject) {
        GameObject tileObject = Object.Instantiate(newTileObject, tile.GetSpawnPoint().position, tile.objectRotation, tile.transform);
        tile.tileObject = tileObject.GetComponent<TileObject>();
    }
}