using UnityEngine;

public class Tile : MonoBehaviour {
    
    [Header("Tile")]
    public TileBlueprint tile;
    public GameObject spawnPoint;
    public GameObject selectEffect;
    
    // Tile
    public enum Type {
        Empty,
        Building,
        Resource
    }
    
    private GameObject _tileObject;
    private Type _tileType;
    private int _resourceWood;
    private int _resourceWaste;
    private int _resourceWhiskey;
    
    // Common
    private UIMenu _uiMenu;
    private BuildManager _buildManager;
    private EnemySpawner _enemySpawner;
    
    #region Unity methods
    
    public void Start() {
        _buildManager = BuildManager.Instance;
        _enemySpawner = EnemySpawner.Instance;
        _resourceWood = tile.resourceWood;
        _resourceWaste = tile.resourceWaste;
        _resourceWhiskey = tile.resourceWhiskey;
        _tileType = tile.type;
        _uiMenu = GetUIMenu();
        
        if (tile.obj != null) {
            _tileObject = Instantiate(tile.obj, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }

    public void OnMouseEnter() {
        selectEffect.SetActive(true);
    }

    public void OnMouseDown() {

        if (_enemySpawner.state == EnemySpawner.State.Fight) {
            return;
        }
        
        if (!_uiMenu) {
            return;
        }

        if (_uiMenu.gameObject.activeSelf) {
            _uiMenu.Deactivate();
        }
        _buildManager.SelectTile(gameObject.GetComponent<Tile>());
        OpenUIMenu();
    }

    public void OnMouseExit() {
        selectEffect.SetActive(false);
    }

    #endregion
    
    
    #region Public class methods

    public void Build(BuildingBlueprint buildingToBuild) {
        CloseMenu();
        ReplaceObject(Type.Building, buildingToBuild.prefab);
    }

    public void Repair() {
        
    }

    public void Collect() {
        
    }

    public void ReplaceObject(Type type, GameObject newObject) {

        if (_tileObject != null) {
            Destroy(_tileObject);
        }
        _tileType = type;
        _uiMenu = GetUIMenu();

        if (newObject == null) {
            return;
        }
        _tileObject = Instantiate(newObject, spawnPoint.transform.position, spawnPoint.transform.rotation);
        Building building = _tileObject?.GetComponent<Building>();

        if (building != null) {
            building.SetParentTile(this);
        }
    }

    public void CloseMenu() {
        _uiMenu.Deactivate();
    }
    
    #endregion

    
    #region Private class methods

    private UIMenu GetUIMenu() {
        
        GameObject menu = _tileType switch {
            Type.Empty => GameObject.Find("BuildMenu"),
            Type.Building => null,
            Type.Resource => null,
            _ => null
        };
        return menu?.GetComponent<UIMenu>();
    }
    
    private void OpenUIMenu() {
        _uiMenu.Activate();
    }
    
    #endregion
}