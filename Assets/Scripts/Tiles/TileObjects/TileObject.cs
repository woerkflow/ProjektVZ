using UnityEngine;

public class TileObject : MonoBehaviour {
    
    [SerializeField] private TileObjectBlueprint blueprint;
    [SerializeField] private Resources resources;
    
    private Tile _parentTile;
    
    
    #region Public Methods
    
    public TileObjectType GetTileObjectType() => blueprint.type;
    
    public GameObject GetPrefab() => blueprint.prefab;
    
    public GameObject GetRuin() => blueprint.ruin;
    
    public Resources GetResources() => resources;
    
    public TileObject GetUpgradePrefab() => blueprint.buildingUpgradePrefab;

    public Tile GetParentTile() => _parentTile;

    public void SetParentTile(Tile tile) {
        _parentTile = tile;
    }

    #endregion
}