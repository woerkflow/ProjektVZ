using UnityEngine;

public class Tile : MonoBehaviour
{

    [Header("Tile")] 
    public TileObject tileObject;
    public GameObject spawnPoint;
    public GameObject selectEffect;
    
    [HideInInspector]
    public int ResourceWood { get; set; }
    public int ResourceWaste { get; set; }
    public int ResourceWhiskey { get; set; }
    
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
    private EnemySpawner _enemySpawner;
    
    
    #region Unity methods
    
    public void Start() {
        _tileType = tileObject.blueprint.type;
        ResourceWood = tileObject.blueprint.resourceWood;
        ResourceWaste = tileObject.blueprint.resourceWaste;
        ResourceWhiskey = tileObject.blueprint.resourceWhiskey;
        
        if (tileObject.blueprint.prefab == null) {
            return;
        }
        _objectRotation = spawnPoint.transform.rotation;
        _tileObject = Instantiate(tileObject.blueprint.prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        _buildManager = BuildManager.Instance;
        _farmManager = FarmManager.Instance;
        _enemySpawner = EnemySpawner.Instance;
    }

    public void OnMouseEnter() {
        selectEffect.SetActive(true);
    }

    public void OnMouseDown() {
        
        if (_enemySpawner.state == EnemySpawner.State.Fight) {
            return;
        }
        
        switch (_tileType) {
            case Type.Empty:
                _buildManager.SelectTile(gameObject.GetComponent<Tile>());
                break;
            case Type.Resource:
                _farmManager.SelectTile(gameObject.GetComponent<Tile>());
                break;
            case Type.Building:
                // Todo: Implement upgrade manager  
                break;
        }
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

    public void RotateObject(float value) {
        Vector3 objectRotationEuler = Quaternion.Normalize(_objectRotation).eulerAngles;
        _objectRotation = Quaternion.Euler(0f, objectRotationEuler.y + value, 0f);
    }
    
    public void ReplaceObject(TileObject newObject) {

        if (_tileObject != null) {
            Destroy(_tileObject);
        }

        if (newObject == null) {
            _tileType = Type.Empty;
            return;
        }
        tileObject = newObject;
        _tileType = tileObject.blueprint.type;
        ResourceWaste += tileObject.blueprint.resourceWaste;
        ResourceWood += tileObject.blueprint.resourceWood;
        ResourceWhiskey += tileObject.blueprint.resourceWhiskey;
        _tileObject = Instantiate(tileObject.blueprint.prefab, spawnPoint.transform.position, _objectRotation);
    }
    
    #endregion
}