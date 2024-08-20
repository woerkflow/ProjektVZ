using UnityEngine;

public class RuinStrategy : ITileReplacementStrategy {
    
    public void ReplaceTileObject(Tile tile, GameObject newTileObject) {
        GameObject tileObject = Object.Instantiate(newTileObject, tile.spawnPoint.transform.position, tile.objectRotation, tile.transform);
        tile.tileObject = tileObject.GetComponent<TileObject>();
    }
}