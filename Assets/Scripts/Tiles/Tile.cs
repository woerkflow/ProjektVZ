using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour {
    
    [Header("Common")]
    public GameObject spawnPoint;
    public GameObject selectEffect;
    public bool isPlayerHouse;
    
    public Resources resources { get; set; }
    public EnemySpawner enemySpawner { get; set; }
    public PlayerManager playerManager { get; set; }
    public MenuManager menuManager { get; set; }
    
    [Header("Interactions")]
    public GameObject replaceEffect;
    public AudioClip buildAudioClip;
    public AudioClip destroyAudioClip;
    public AudioClip farmAudioClip;
    public AudioClip repairAudioClip;
    public AudioClip upgradeAudioClip;
    
    public SoundFXManager soundFXManager { get; set; }
    
    private Dictionary<TileInteractionType, ITileInteractionStrategy> _tileInteractionStrategies;
    
    [Header("Tile Object")]
    public TileObjectType type;
    public TileObject startObject;
    public TileObject[] randomWood;
    public TileObject[] randomWaste;
    
    public TileObject tileObject { get; set; }
    public Building tileObjectBuilding { get; set; }
    public Quaternion objectRotation { get; set; }
    public TileObject selectedBuilding { get; set; }
    
    private Dictionary<TileObjectType, ITileReplacementStrategy> _tileReplacementStrategies;
    
    
    #region Unity Methods
    
    private void Start() {
        CacheManagers();
        InitializeStrategies();
        objectRotation = spawnPoint.transform.rotation;
        ClearResources();
        InitializeTileObject();
    }
    
    public void OnMouseEnter() {
        selectEffect.SetActive(true);
        selectEffect.transform.position = spawnPoint.transform.position;
    }
    
    public void OnMouseDown() {
        menuManager.CloseMenus();
        
        if (isPlayerHouse || enemySpawner.state.GetType().ToString() == "FightState") {
            return;
        }
        menuManager.OpenMenu(this);
    }
    
    public void OnMouseExit() {
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
        soundFXManager.PlaySoundFXClip(audioClip, transform.position, 0.5f);
    }
    
    public void PlayEffect(GameObject effect) {
        GameObject effectInstance = Instantiate(effect.gameObject, transform.position, effect.transform.rotation);
        Destroy(effectInstance, effect.GetComponent<ParticleSystem>().main.duration);
    }
    
    #endregion
    
    
    #region Tile Object Management Methods
    
    public static float GetRandomRotation()
        => Random.Range(0, 4) * 90f;
    
    public void RotateObject(float value) {
        Vector3 objectRotationEuler = objectRotation.eulerAngles;
        objectRotation = Quaternion.Euler(0f, objectRotationEuler.y + value, 0f);
    }

    public static void DestroyTileObject(TileObject tileObject) {
        Destroy(tileObject.gameObject);
    }

    public void DestroyObject() {
        TileObject ruin = tileObject.blueprint.ruin?.GetComponent<TileObject>();
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
        type = newObject.blueprint.type;
        
        if (!_tileReplacementStrategies.TryGetValue(type, out ITileReplacementStrategy strategy)) {
            return;
        }
        strategy.ReplaceTileObject(this, newObject.blueprint.prefab);
        
        tileObject.parentTile = this;
        objectRotation = spawnPoint.transform.rotation;
    }
    
    #endregion
    
    
    #region Resource Management Methods
    
    public Resources GetPlayerResources() => playerManager.resources;
    
    public static Resources GetRepairCosts(TileObject tileObject, Building building) {
        float costFactor = 1 - building.currentHealth / building.maxHealth;
        return new Resources {
            wood = (int) Mathf.Floor(tileObject.blueprint.resources.wood * costFactor),
            waste = (int) Mathf.Floor(tileObject.blueprint.resources.waste * costFactor),
            whiskey = (int) Mathf.Floor(tileObject.blueprint.resources.whiskey * costFactor)
        };
    }
    
    public void AddResources(Resources resourcesToAdd) {
        resources = new Resources {
            wood = resources.wood + resourcesToAdd.wood,
            waste = resources. waste + resourcesToAdd.waste,
            whiskey = resources.whiskey + resourcesToAdd.whiskey
        };
    }
    
    public bool HasResources(Resources requiredResources)
        => resources.wood >= requiredResources.wood &&
           resources.waste >= requiredResources.waste &&
           resources.whiskey >= requiredResources.whiskey;
    
    public void ClearResources() {
        resources = new Resources {
            wood = 0,
            waste = 0,
            whiskey = 0
        };
    }
    
    #endregion
    
    
    #region Private Methods
    
    private static TileObject GetRandomResource(TileObject[] randomWood, TileObject[] randomWaste)
        => Random.Range(0, 10) == 0
            ? Random.Range(0, 6) switch {
                0 => randomWaste[0],
                > 0 and < 3 => randomWaste[1],
                _ => randomWaste[2]
            }
            : Random.Range(0, 6) switch {
                0 => randomWood[Random.Range(0, 2)],
                > 0 and < 3 => randomWood[Random.Range(2, 4)],
                _ => randomWood[Random.Range(4, 6)]
            };

    private void InitializeTileObject() {
        
        if (type == TileObjectType.Resource) {
            startObject = GetRandomResource(randomWood, randomWaste);
        }
        ReplaceObject(startObject);
    }
    
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
    
    private void CacheManagers() {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        menuManager = FindObjectOfType<MenuManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        soundFXManager = FindObjectOfType<SoundFXManager>();
    }
    
    #endregion
}