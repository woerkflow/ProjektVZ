using TMPro;

public class HoverBuildingMenu : UIMenu {

    public TMP_Text label;
    public TMP_Text value;

    private TileObjectType _tileType;

    public void SetValue(Tile tile) {
        Building building = tile.tileObjectBuilding;
        label.SetText("Health:");
        value.SetText(building.currentHealth + "/" + building.maxHealth);
    }
}