using UnityEngine;

public class Tile : MonoBehaviour {

    [Header("Tile")] 
    public TileObject tileObject;
    public GameObject spawnPoint;
    public GameObject selectEffect;
    public TileObject[] randomObjects;
    public bool isBlocked;
    
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
        _buildManager = BuildManager.Instance;
        _farmManager = FarmManager.Instance;
        _enemySpawner = EnemySpawner.Instance;
        _objectRotation = spawnPoint.transform.rotation;

        if (!isBlocked) {
            tileObject = GetRandomObject();
            _objectRotation = GetRandomRotation();
        }
        ReplaceObject(tileObject);
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
        ReplaceObject(buildingToBuild);
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
            
            TransformTile(
                Type.Empty,
                0,
                0,
                0
            );
            return;
        }
        tileObject = newObject;
        
        TransformTile(
            tileObject.blueprint.type,
            tileObject.blueprint.resourceWood,
            tileObject.blueprint.resourceWaste,
            tileObject.blueprint.resourceWhiskey
        );

        _tileObject = 
            Instantiate(
                tileObject.blueprint.prefab, 
                spawnPoint.transform.position, 
                _objectRotation
            );

        _tileObject.GetComponent<TileObject>().parentTile = this;
    }
    
    #endregion
    
    
    #region Private class Methods

    private void TransformTile(Type type, int wood, int waste, int whiskey) {
        _tileType = type;
        ResourceWood = wood;
        ResourceWaste = waste;
        ResourceWhiskey = whiskey;
    }

    private TileObject GetRandomObject() {
        return randomObjects[Random.Range(0, randomObjects.Length)];
    }

    private Quaternion GetRandomRotation() {
        float randomDegrees = Random.Range(0, 4) * 90f;
        return Quaternion.Euler(0f, randomDegrees, 0f);
    }
    
    #endregion
}