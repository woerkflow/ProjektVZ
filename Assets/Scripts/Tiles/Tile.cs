using UnityEngine;

public class Tile : MonoBehaviour {
    
    [Header("Tile")]
    public TileObject tileObject;
    public GameObject spawnPoint;
    public GameObject selectEffect;
    
    public enum Type {
        Empty,
        Building,
        Resource
    }
    
    // Tile
    private GameObject _tileObject;
    private Type _tileType;
    private int _resourceWood;
    private int _resourceWaste;
    private int _resourceWhiskey;
    
    // Common
    private BuildManager _buildManager;
    private EnemySpawner _enemySpawner;
    
    #region Unity methods
    
    public void Start() {
        _buildManager = BuildManager.Instance;
        _enemySpawner = EnemySpawner.Instance;
        
        _tileType = tileObject.blueprint.type;
        _resourceWood = tileObject.blueprint.resourceWood;
        _resourceWaste = tileObject.blueprint.resourceWaste;
        _resourceWhiskey = tileObject.blueprint.resourceWhiskey;
        
        if (tileObject.blueprint.prefab != null) {
            _tileObject = Instantiate(tileObject.blueprint.prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }

    public void OnMouseEnter() {
        selectEffect.SetActive(true);
    }

    public void OnMouseDown() {

        if (_enemySpawner.state == EnemySpawner.State.Fight) {
            return;
        }
        _buildManager.SelectTile(gameObject.GetComponent<Tile>());
    }

    public void OnMouseExit() {
        selectEffect.SetActive(false);
    }

    #endregion
    
    
    #region Public class methods

    public void Build(Building buildingToBuild) {
        
        // Replace object
        ReplaceObject(buildingToBuild);

        if (buildingToBuild != null) {
            buildingToBuild.SetParentTile(this);
        }
    }

    public void Repair() {
        
    }

    public void Collect() {
        
    }

    public void ReplaceObject(TileObject newObject) {

        if (_tileObject != null) {
            Destroy(_tileObject);
        }

        if (newObject == null) {
            return;
        }
        tileObject = newObject;
        _tileType = tileObject.blueprint.type;
        _resourceWaste += tileObject.blueprint.resourceWaste;
        _resourceWood += tileObject.blueprint.resourceWood;
        _resourceWhiskey += tileObject.blueprint.resourceWhiskey;
        _tileObject = Instantiate(tileObject.blueprint.prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }
    
    #endregion
}