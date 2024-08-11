using UnityEngine;

public class Tile : MonoBehaviour {

    [Header("Tile")] 
    public TileObject startObject;
    public GameObject spawnPoint;
    public GameObject selectEffect;
    public TileObject[] randomObjects;
    public bool isBlocked;
    
    public enum TileType {
        Empty,
        Building,
        Resource
    }

    private Resources _resources;
    private GameObject _tileObject;
    private Quaternion _objectRotation;
    private TileType _tileType;
    private BuildManager _buildManager;
    private FarmManager _farmManager;
    private UpgradeManager _upgradeManager;
    private EnemySpawner _enemySpawner;


    #region Unity Methods

    private void Start() {
        CacheManagers();
        _objectRotation = spawnPoint.transform.rotation;

        if (!isBlocked) {
            startObject = GetRandomObject(randomObjects);
            RotateObject(GetRandomRotation());
        }
        ReplaceObject(startObject);
    }

    public void OnMouseEnter() {
        selectEffect.SetActive(true);
        selectEffect.transform.position = spawnPoint.transform.position;
    }

    public void OnMouseDown() {

        if (_enemySpawner.state == EnemySpawner.State.Fight) {
            return;
        }
        CloseActiveMenus();

        switch (_tileType) {
            case TileType.Empty:
                _buildManager.SelectTile(this);
                _buildManager.ActivateMenu();
                break;
            case TileType.Resource:
                _farmManager.SelectTile(this);
                _farmManager.ActivateMenu();
                break;
            case TileType.Building:
                _upgradeManager.SelectTile(this);
                _upgradeManager.ActivateMenu();
                break;
        }
    }

    public void OnMouseExit() {
        selectEffect.SetActive(false);
        selectEffect.transform.position = Vector3.zero;
    }

    #endregion

    
    #region Public Methods

    public void Build(Building buildingToBuild) {
        ReplaceObject(buildingToBuild);
    }

    public TileObject GetTileObject() 
        => _tileObject?.GetComponent<TileObject>();

    public void RotateObject(float value) {
        Vector3 objectRotationEuler = _objectRotation.eulerAngles;
        _objectRotation = Quaternion.Euler(0f, objectRotationEuler.y + value, 0f);
    }
    
    public void ReplaceObject(TileObject newObject) {
        
        if (_tileObject) {
            
            if (GetTileObject()?.blueprint.type == TileType.Building) {
                _buildManager.RemoveBuilding(_tileObject);
            }
            Destroy(_tileObject);
        }
        InitializeTile(newObject.blueprint);
        _tileObject = Instantiate(newObject.blueprint.prefab, spawnPoint.transform.position, _objectRotation, transform);

        if (newObject.blueprint.type == TileType.Building) {
            _buildManager.AddBuilding(_tileObject);
        }
        GetTileObject()?.SetParentTile(this);
        _objectRotation = spawnPoint.transform.rotation;
    }

    public Resources GetResources() 
        => _resources;
    
    public void AddResources(Resources resourcesToAdd) {
        _resources.wood += resourcesToAdd.wood;
        _resources.waste += resourcesToAdd.waste;
        _resources.whiskey += resourcesToAdd.whiskey;
    }

    public bool HasResources(Resources requiredResources)
        => _resources.wood >= requiredResources.wood &&
           _resources.waste >= requiredResources.waste &&
           _resources.whiskey >= requiredResources.whiskey;

    public void ClearResources() {
        _resources.wood = 0;
        _resources.waste = 0;
        _resources.whiskey = 0;
    }

    #endregion

    
    #region Private Methods
    
    private static TileObject GetRandomObject(TileObject[] randomObjects)
        => randomObjects[Random.Range(0, randomObjects.Length)];

    private static float GetRandomRotation() 
        => Random.Range(0, 4) * 90f;

    private void CacheManagers() {
        _buildManager = BuildManager.Instance;
        _farmManager = FarmManager.Instance;
        _upgradeManager = UpgradeManager.Instance;
        _enemySpawner = EnemySpawner.Instance;
    }
    
    private void InitializeTile(TileObjectBlueprint blueprint) {
        _tileType = blueprint.type;
        _resources.wood = blueprint.resourceWood;
        _resources.waste = blueprint.resourceWaste;
        _resources.whiskey = blueprint.resourceWhiskey;
    }

    private void CloseActiveMenus() {
        
        if (_buildManager.MenuIsActive()) {
            _buildManager.CloseMenu();
        }
        
        if (_farmManager.MenuIsActive()) {
            _farmManager.CloseMenu();
        }
        
        if (_upgradeManager.MenuIsActive()) {
            _upgradeManager.CloseMenu();
        }
    }

    #endregion
}