using UnityEngine;

public class Tile : MonoBehaviour {

    [Header("Tile")] 
    public TileObject startObject;
    public GameObject spawnPoint;
    public GameObject selectEffect;
    public TileObject[] randomObjects;
    public bool isBlocked;
    
    [HideInInspector]
    public int resourceWood { get; set; }
    public int resourceWaste { get; set; }
    public int resourceWhiskey { get; set; }
    
    public enum Type {
        Empty,
        Building,
        Resource
    }
    
    // Tile
    private GameObject _tileObject;
    private Quaternion _objectRotation;
    private Type _tileType;
    
    // Common
    private BuildManager _buildManager;
    private FarmManager _farmManager;
    private UpgradeManager _upgradeManager;
    private EnemySpawner _enemySpawner;
    
    
    #region Unity methods
    
    private void Start() {
        _buildManager = BuildManager.Instance;
        _farmManager = FarmManager.Instance;
        _upgradeManager = UpgradeManager.Instance;
        _enemySpawner = EnemySpawner.Instance;
        _objectRotation = spawnPoint.transform.rotation;

        if (!isBlocked) {
            startObject = GetRandomObject(randomObjects);
            _objectRotation = GetRandomRotation();
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

        if (_buildManager.MenuIsActive()) {
            _buildManager.CloseMenu();
        }
        
        if (_farmManager.MenuIsActive()) {
            _farmManager.CloseMenu();
        }

        if (_upgradeManager.MenuIsActive()) {
            _upgradeManager.CloseMenu();
        }
        
        switch (_tileType) {
            case Type.Empty:
                _buildManager.SelectTile(gameObject.GetComponent<Tile>());
                _buildManager.ActivateMenu();
                break;
            case Type.Resource:
                _farmManager.SelectTile(gameObject.GetComponent<Tile>());
                _farmManager.ActivateMenu();
                break;
            case Type.Building:
                _upgradeManager.SelectTile(gameObject.GetComponent<Tile>());
                _upgradeManager.ActivateMenu();
                break;
        }
    }

    public void OnMouseExit() {
        selectEffect.SetActive(false);
        selectEffect.transform.position = Vector3.zero;
    }

    #endregion
    
    
    #region Public class methods

    public void Build(Building buildingToBuild) {
        ReplaceObject(buildingToBuild);
    }

    public TileObject GetTileObject() {
        return _tileObject.GetComponent<TileObject>();
    }

    public void RotateObject(float value) {
        Vector3 objectRotationEuler = Quaternion.Normalize(_objectRotation).eulerAngles;
        _objectRotation = Quaternion.Euler(0f, objectRotationEuler.y + value, 0f);
    }
    
    public void ReplaceObject(TileObject newObject) {

        if (_tileObject != null) {
            
            // Remove building from building list
            if (GetTileObject().blueprint.type == Type.Building) {
                _buildManager.RemoveBuilding(_tileObject);
            }
            
            // Destroy tile object
            Destroy(_tileObject);
        }
        
        TransformTile(
            newObject.blueprint.type,
            newObject.blueprint.resourceWood,
            newObject.blueprint.resourceWaste,
            newObject.blueprint.resourceWhiskey
        );

        _tileObject = 
            Instantiate(
                newObject.blueprint.prefab, 
                spawnPoint.transform.position, 
                _objectRotation,
                gameObject.transform
            );
        
        // Reset rotation
        _objectRotation = spawnPoint.transform.rotation;
        
        // Add building to building list
        if (newObject.blueprint.type == Type.Building) {
            _buildManager.AddBuilding(_tileObject);
        }
        
        // Set this tile as parent tile of tile object
        GetTileObject().SetParentTile(this);
    }
    
    #endregion
    
    
    #region Private class Methods

    private void TransformTile(Type type, int wood, int waste, int whiskey) {
        _tileType = type;
        resourceWood = wood;
        resourceWaste = waste;
        resourceWhiskey = whiskey;
    }

    private static TileObject GetRandomObject(TileObject[] randomObjects) {
        return randomObjects[Random.Range(0, randomObjects.Length)];
    }

    private static Quaternion GetRandomRotation() {
        float randomDegrees = Random.Range(0, 4) * 90f;
        return Quaternion.Euler(0f, randomDegrees, 0f);
    }
    
    #endregion
}