using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour {

    [SerializeField] private TileBlueprint blueprint;
    
    public EnemySpawner enemySpawner { get; private set; }
    public PlayerManager playerManager { get; private set; }
    public JobSystemManager jobSystemManager { get; private set; }
    public TileObject tileObject { get; set; }
    public Building tileObjectBuilding { get; set; }
    public TileObject selectedBuilding { get; private set; }
    
    private MenuManager _menuManager;
    private FXManager _fxManager;
    private Dictionary<TileInteractionType, ITileInteractionStrategy> _tileInteractionStrategies;
    private Dictionary<TileObjectType, ITileReplacementStrategy> _tileReplacementStrategies;
    
    
    #region Unity Methods
    
    private void Start() {
        InitializeManagers();
        InitializeStrategies();
        ReplaceObject(blueprint.startObject);
    }
    
    #endregion


    #region Tile Interaction Methods
    
    public void OnRayEnter() {
        _menuManager.OpenHoverMenu(this);
        blueprint.selectEffect.SetActive(true);
        blueprint.selectEffect.transform.position = blueprint.spawnPoint.transform.position;
    }
    
    public void OnRayDown() {
        _menuManager.CloseMenus();
        _menuManager.CloseHoverMenus();
        
        if (blueprint.isPlayerHouse || enemySpawner.state.GetType().ToString() == "FightState") {
            return;
        }

        if (blueprint.type == TileObjectType.Resource) {
            PerformInteraction(TileInteractionType.Farm);
            return;
        }
        _menuManager.OpenMenu(this);
    }
    
    public void OnRayExit() {
        _menuManager.CloseHoverMenus();
        blueprint.selectEffect.SetActive(false);
        blueprint.selectEffect.transform.position = Vector3.zero;
    }
    
    #endregion
    
    
    #region Tile Interaction Strategy Methods
    
    public void OnSelect(TileObject buildingToBuild) {
        selectedBuilding = buildingToBuild;
    }
    
    public bool PerformInteraction(TileInteractionType strategyType) {
        
        if (!_tileInteractionStrategies.TryGetValue(strategyType, out ITileInteractionStrategy strategy)
            || !strategy.CanInteract(this)
        ) {
            return false;
        }
        strategy.Interact(this);
        return true;
    }

    public void PlaySound(AudioClip audioClip) {
        _fxManager.PlaySound(
            audioClip, 
            transform.position, 
            0.5f
        );
    }
    
    public void PlayEffect(GameObject effect) {
        _fxManager.PlayEffect(
            effect, 
            transform.position,
            effect.transform.rotation,
            gameObject.transform
        );
    }
    
    #endregion
    
    
    #region Tile Object Management Methods
    
    public Quaternion GetRotation() 
        => blueprint.objectRotation;
    
    public static float GetRandomRotation()
        => Random.Range(0, 4) * 90f;
    
    public void RotateObject(float value) {
        Vector3 objectRotationEuler = blueprint.objectRotation.eulerAngles;
        blueprint.objectRotation = Quaternion.Euler(0f, objectRotationEuler.y + value, 0f);
    }

    public static void DestroyTileObject(TileObject tileObject) {
        Destroy(tileObject.gameObject, 0.1f);
    }

    public void DestroyObject() {
        
        if (blueprint.isPlayerHouse) {
            PlayerManager.LoadMainMenu();
        }
        TileObject ruin = tileObject.GetRuin()?.GetComponent<TileObject>();
        blueprint.objectRotation = tileObject.transform.rotation;
        
        if (blueprint.type == TileObjectType.Building) {
            jobSystemManager.UnregisterBuilding(tileObjectBuilding);
            tileObjectBuilding = null;
        }
        DestroyTileObject(tileObject);
        
        if (!ruin) {
            return;
        }
        ReplaceObject(ruin);
    }
    
    public void ReplaceObject(TileObject newObject) {
        blueprint.type = newObject.GetTileObjectType();
        
        if (!_tileReplacementStrategies.TryGetValue(blueprint.type, out ITileReplacementStrategy strategy)) {
            return;
        }
        strategy.ReplaceTileObject(this, newObject.GetPrefab());
        
        tileObject.SetParentTile(this);
        blueprint.objectRotation = blueprint.spawnPoint.transform.rotation;
    }
    
    #endregion
    
    
    #region Resource Management Methods
    
    public static Resources GetRepairCosts(TileObject tileObject, Building building) {
        float costFactor = 1 - (float) building.currentHealth / building.GetMaxHealth();
        return new Resources {
            wood = (int) Mathf.Floor(tileObject.GetResources().wood * costFactor),
            waste = (int) Mathf.Floor(tileObject.GetResources().waste * costFactor),
            whiskey = (int) Mathf.Floor(tileObject.GetResources().whiskey * costFactor)
        };
    }
    
    #endregion
    
    
    #region Public Methods

    public Transform GetSpawnPoint() => blueprint.spawnPoint;

    public GameObject GetReplaceEffect() => blueprint.replaceEffect;
    
    public AudioClip GetBuildAudioClip() => blueprint.buildAudioClip;

    public AudioClip GetDestroyAudioClip() => blueprint.destroyAudioClip;

    public AudioClip GetFarmAudioClip() => blueprint.farmAudioClip;

    public AudioClip GetRepairAudioClip() => blueprint.repairAudioClip;

    public AudioClip GetUpgradeAudioClip() => blueprint.upgradeAudioClip;
    
    public TileObjectType GetTileObjectType() => blueprint.type;
    
    #endregion
    
    
    #region Private Methods
    
    public TileObject GetRandomResource()
        => Random.Range(0, 20) <= 2
            ? Random.Range(0, 10) switch {
                0 => blueprint.randomWaste[0],
                >= 1 and <= 3 => blueprint.randomWaste[1],
                _ => blueprint.randomWaste[2]
            }
            : Random.Range(0, 10) switch {
                0 => blueprint.randomWood[Random.Range(0, 2)],
                >= 1 and <= 3 => blueprint.randomWood[Random.Range(2, 4)],
                _ => blueprint.randomWood[Random.Range(4, 6)]
            };
    
    private void InitializeStrategies() {
        _tileInteractionStrategies = new Dictionary<TileInteractionType, ITileInteractionStrategy> {
            { TileInteractionType.Build, new BuildStrategy() },
            { TileInteractionType.Repair, new RepairStrategy() },
            { TileInteractionType.Destroy, new DestroyStrategy() },
            { TileInteractionType.Farm, new FarmStrategy() },
            { TileInteractionType.Upgrade, new UpgradeStrategy() }
        };
        _tileReplacementStrategies = new Dictionary<TileObjectType, ITileReplacementStrategy> {
            { TileObjectType.Building, new BuildingStrategy() },
            { TileObjectType.Empty, new EmptyStrategy() },
            { TileObjectType.Resource, new ResourceStrategy() },
            { TileObjectType.Ruin, new RuinStrategy() }
        };
    }
    
    private void InitializeManagers() {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        _menuManager = FindObjectOfType<MenuManager>();
        jobSystemManager = FindObjectOfType<JobSystemManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        _fxManager = FindObjectOfType<FXManager>();
    }
    
    #endregion
}