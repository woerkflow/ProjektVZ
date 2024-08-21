using TMPro;
using UnityEngine;

public class HoverBuildingMenu : UIMenu {

    public TMP_Text label;
    public TMP_Text value;

    private TileObjectType _tileType;

    public void SetValue(Tile tile) {
        Building building = tile.tileObjectBuilding;
        label.SetText("Health:");
        value.SetText(building.currentHealth + "/" + building.maxHealth);
    }

    public void SetPosition(Tile tile) {
        transform.position =
            new Vector3(
                tile.spawnPoint.transform.position.x,
                tile.spawnPoint.transform.position.y + 0.05f,
                tile.spawnPoint.transform.position.z
            );
    }
}