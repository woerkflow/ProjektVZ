using TMPro;

public class HoverResourceMenu : UIMenu {

    public TMP_Text[] labels;
    public TMP_Text[] values;

    private TileObjectType _tileType;

    public void SetValues(Tile tile) {
        Resources tileObjectResources = tile.tileObject.blueprint.resources;
        labels[0].SetText(GetLabelText(tileObjectResources));
        labels[1].SetText("Time:");
        values[0].SetText(GetValueText(tileObjectResources));
        values[1].SetText("-" + tile.tileObject.blueprint.timeCosts + "s");
    }

    private static string GetLabelText(Resources resources) 
        => resources.wood > 0 
            ? "Wood:" 
            : resources.waste > 0 
                ? "Waste:" 
                : "Whiskey:";
    
    private static string GetValueText(Resources resources)
        => resources.wood > 0 
            ? "+" + resources.wood 
            : resources.waste > 0 
                ? "+" + resources.waste 
                : "+" + resources.whiskey;
}