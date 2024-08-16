using UnityEngine;

[System.Serializable]
public struct TileObjectBlueprint {
    public TileObjectType type;
    public GameObject prefab;
    public GameObject ruin;
    public int timeCosts;
    public Resources resources;
    public TileObject buildingUpgradePrefab;
}