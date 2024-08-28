using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour {
    
    [Header("Common")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject selectEffect;
    [SerializeField] private bool isPlayerHouse;
    
    public EnemySpawner enemySpawner { get; private set; }
    public PlayerManager playerManager { get; private set; }

    private MenuManager _menuManager;
    
    [Header("Interactions")]
    [SerializeField] private GameObject replaceEffect;
    [SerializeField] private AudioClip buildAudioClip;
    [SerializeField] private AudioClip destroyAudioClip;
    [SerializeField] private AudioClip farmAudioClip;
    [SerializeField] private AudioClip repairAudioClip;
    [SerializeField] private AudioClip upgradeAudioClip;

    private FXManager _fxManager;
    private Dictionary<TileInteractionType, ITileInteractionStrategy> _tileInteractionStrategies;
    
    [Header("Tile Object")]
    [SerializeField] private TileObjectType type;
    [SerializeField] private TileObject startObject;
    [SerializeField] private Quaternion objectRotation;
    [SerializeField] private TileObject[] randomWood;
    [SerializeField] private TileObject[] randomWaste;
    
    public TileObject tileObject { get; set; }
    public Building tileObjectBuilding { get; set; }
    public TileObject selectedBuilding { get; private set; }
    
    private Dictionary<TileObjectType, ITileReplacementStrategy> _tileReplacementStrategies;

    
    #region Unity Methods
    
    private void Start() {
        InitializeManagers();
        InitializeStrategies();
        ReplaceObject(startObject);
    }
    
    #endregion


    #region Tile Interaction Methods
    
    public void OnRayEnter() {
        _menuManager.OpenHoverMenu(this);
        selectEffect.SetActive(true);
        selectEffect.transform.position = spawnPoint.transform.position;
    }
    
    public void OnRayDown() {
        _menuManager.CloseMenus();
        _menuManager.CloseHoverMenus();
        
        if (isPlayerHouse || enemySpawner.state.GetType().ToString() == "FightState") {
            return;
        }

        if (type == TileObjectType.Resource) {
            PerformInteraction(TileInteractionType.Farm);
            return;
        }
        _menuManager.OpenMenu(this);
    }
    
    public void OnRayExit() {
        _menuManager.CloseHoverMenus();
        selectEffect.SetActive(false);
        selectEffect.transform.position = Vector3.zero;
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
            effect.transform.rotation
        );
    }
    
    #endregion
    
    
    #region Tile Object Management Methods
    
    public Quaternion GetRotation() 
        => objectRotation;
    
    public static float GetRandomRotation()
        => Random.Range(0, 4) * 90f;
    
    public void RotateObject(float value) {
        Vector3 objectRotationEuler = objectRotation.eulerAngles;
        objectRotation = Quaternion.Euler(0f, objectRotationEuler.y + value, 0f);
    }

    public static void DestroyTileObject(TileObject tileObject) {
        Destroy(tileObject.gameObject, 0.1f);
    }

    public void DestroyObject() {
        
        if (isPlayerHouse) {
            PlayerManager.LoadMainMenu();
        }
        TileObject ruin = tileObject.GetBluePrint().ruin?.GetComponent<TileObject>();
        objectRotation = tileObject.transform.rotation;
        
        if (type == TileObjectType.Building) {
            playerManager.RemoveBuilding(tileObjectBuilding);
            tileObjectBuilding = null;
        }
        DestroyTileObject(tileObject);
        
        if (!ruin) {
            return;
        }
        ReplaceObject(ruin);
    }
    
    public void ReplaceObject(TileObject newObject) {
        type = newObject.GetBluePrint().type;
        
        if (!_tileReplacementStrategies.TryGetValue(type, out ITileReplacementStrategy strategy)) {
            return;
        }
        strategy.ReplaceTileObject(this, newObject.GetBluePrint().prefab);
        
        tileObject.SetParentTile(this);
        objectRotation = spawnPoint.transform.rotation;
    }
    
    #endregion
    
    
    #region Resource Management Methods
    
    public static Resources GetRepairCosts(TileObject tileObject, Building building) {
        float costFactor = 1 - (float) building.currentHealth / building.GetMaxHealth();
        return new Resources {
            wood = (int) Mathf.Floor(tileObject.GetBluePrint().resources.wood * costFactor),
            waste = (int) Mathf.Floor(tileObject.GetBluePrint().resources.waste * costFactor),
            whiskey = (int) Mathf.Floor(tileObject.GetBluePrint().resources.whiskey * costFactor)
        };
    }
    
    #endregion
    
    
    #region Public Methods

    public Transform GetSpawnPoint() => spawnPoint;

    public GameObject GetReplaceEffect() => replaceEffect;
    
    public AudioClip GetBuildAudioClip() => buildAudioClip;

    public AudioClip GetDestroyAudioClip() => destroyAudioClip;

    public AudioClip GetFarmAudioClip() => farmAudioClip;

    public AudioClip GetRepairAudioClip() => repairAudioClip;

    public AudioClip GetUpgradeAudioClip() => upgradeAudioClip;
    
    public TileObjectType GetTileObjectType() => type;
    
    #endregion
    
    
    #region Private Methods
    
    public TileObject GetRandomResource()
        => Random.Range(0, 10) == 0
            ? Random.Range(0, 10) switch {
                0 => randomWaste[0],
                >= 1 and <= 3 => randomWaste[1],
                _ => randomWaste[2]
            }
            : Random.Range(0, 10) switch {
                0 => randomWood[Random.Range(0, 2)],
                >= 1 and <= 3 => randomWood[Random.Range(2, 4)],
                _ => randomWood[Random.Range(4, 6)]
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
        playerManager = FindObjectOfType<PlayerManager>();
        _fxManager = FindObjectOfType<FXManager>();
    }
    
    #endregion
}