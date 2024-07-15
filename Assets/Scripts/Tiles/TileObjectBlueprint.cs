using UnityEngine;

[System.Serializable]
public class TileObjectBlueprint {
    
    public Tile.Type type;
    public GameObject prefab;
    public GameObject ruin;
    public int timeCosts;
    public int resourceWood;
    public int resourceWaste;
    public int resourceWhiskey;
}