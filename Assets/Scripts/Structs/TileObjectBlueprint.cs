using UnityEngine;

[System.Serializable]
public struct TileObjectBlueprint {
    public TileObjectType type;
    public GameObject prefab;
    public GameObject ruin;
    public TileObject buildingUpgradePrefab;
}