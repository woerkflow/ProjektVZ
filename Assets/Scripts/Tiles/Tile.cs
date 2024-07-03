using UnityEngine;

public class Tile : MonoBehaviour {
    
    [Header("Tile")]
    public TileBlueprint tile;
    public GameObject spawnPoint;
    public GameObject selectEffect;
    
    // Tile
    private GameObject _tileObject;
    private int _resourceWood;
    private int _resourceWaste;
    private int _resourceWhiskey;
    
    // Common
    private UIMenu _uiMenu;
    private BuildManager _buildManager;
    
    
    #region Unity methods
    
    public void Start() {
        _buildManager = BuildManager.Instance;
        _resourceWood = tile.resourceWood;
        _resourceWaste = tile.resourceWaste;
        _resourceWhiskey = tile.resourceWhiskey;

        GameObject menu = tile.type switch {
            "Empty" => GameObject.Find("BuildMenu"),
            "Building" => null,
            "Resource" => null,
            _ => null
        };
        _uiMenu = menu?.GetComponent<UIMenu>();
        
        
        if (tile.obj != null) {
            _tileObject = Instantiate(tile.obj, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }

    public void OnMouseEnter() {
        selectEffect.SetActive(true);
    }

    public void OnMouseDown() {
        
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
        Destroy(_tileObject);
        _tileObject = Instantiate(buildingToBuild.prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        Building building = _tileObject.GetComponent<Building>();
        building.SetParentTile(this);
    }

    public void Repair() {
        
    }

    public void Collect() {
        
    }

    public void Destroy(GameObject destroyedObject) {

        if (_tileObject != null) {
            Destroy(_tileObject);
        }

        if (destroyedObject != null) {
            _tileObject = Instantiate(destroyedObject, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }

    public void CloseMenu() {
        _uiMenu.Deactivate();
    }
    
    #endregion

    
    #region Private class methods

    private void OpenUIMenu() {
        _uiMenu.Activate();
    }
    
    #endregion
}