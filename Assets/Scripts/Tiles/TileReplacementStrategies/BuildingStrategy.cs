using UnityEngine;

public class BuildingStrategy : ITileReplacementStrategy {
    
    public void ReplaceTileObject(Tile tile, GameObject newTileObject) {
        GameObject tileObject = Object.Instantiate(newTileObject, tile.GetSpawnPoint().position, tile.objectRotation, tile.transform);
        tile.tileObject = tileObject.GetComponent<TileObject>();
        tile.tileObjectBuilding = tileObject.gameObject.GetComponent<Building>();
        tile.playerManager.AddBuilding(tile.tileObjectBuilding);
    }
}
