using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour {
    
    [Header("Common")]
    public GameObject spawnPoint;
    public GameObject selectEffect;
    public GameObject replaceEffect;
    public bool isBlocked;
    
    public Resources resources { get; set; }
    public EnemySpawner enemySpawner { get; set; }
    public PlayerManager playerManager { get; set; }
    public MenuManager menuManager { get; set; }
    
    private Dictionary<TileStrategyType, ITileStrategy> _tileStrategies;
    
    [Header("Tile Object")]
    public TileObjectType type;
    public TileObject startObject;
    public TileObject[] randomWood;
    public TileObject[] randomWaste;
    
    public TileObject tileObject { get; set; }
    public Building tileObjectBuilding { get; set; }
    public Quaternion objectRotation { get; set; }
    public TileObject selectedBuilding { get; set; }
    
    
    #region Unity Methods
    
    private void Start() {
        CacheManagers();
        objectRotation = spawnPoint.transform.rotation;

        if (!isBlocked) {
            startObject = GetRandomResource(randomWood, randomWaste);
            RotateObject(GetRandomRotation());
        }
        ReplaceObject(startObject, false);

        InitializeStrategies();
    }
    
    public void OnMouseEnter() {
        selectEffect.SetActive(true);
        selectEffect.transform.position = spawnPoint.transform.position;
    }
    
    public void OnMouseDown() {
        menuManager.CloseMenus();
        
        if (enemySpawner.state.GetType().ToString() == "FightState") {
            return;
        }
        menuManager.OpenMenu(type, this);
    }
    
    public void OnMouseExit() {
        selectEffect.SetActive(false);
        selectEffect.transform.position = Vector3.zero;
    }
    
    #endregion
    
    
    #region Tile Strategy Methods
    
    private void InitializeStrategies() {
        _tileStrategies = new Dictionary<TileStrategyType, ITileStrategy> {
            { TileStrategyType.Build, new BuildStrategy() },
            { TileStrategyType.Repair, new RepairStrategy() },
            { TileStrategyType.Destroy, new DestroyStrategy() },
            { TileStrategyType.Farm, new FarmStrategy() },
            { TileStrategyType.Upgrade, new UpgradeStrategy() }
        };
    }
    
    public void OnSelect(TileObject buildingToBuild) {
        selectedBuilding = buildingToBuild;
    }
    
    public bool PerformInteraction(TileStrategyType strategyType) {
        
        if (!_tileStrategies.TryGetValue(strategyType, out ITileStrategy strategy)
            || !strategy.CanInteract(this)
        ) {
            return false;
        }
        strategy.Interact(this);
        return true;
    }
    
    #endregion
    
    
    #region Common Tile Object Management Methods
    
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
                _ => randomWood[4]
            };
    
    private static float GetRandomRotation() 
        => Random.Range(0, 4) * 90f;
    
    public void RotateObject(float value) {
        Vector3 objectRotationEuler = objectRotation.eulerAngles;
        objectRotation = Quaternion.Euler(0f, objectRotationEuler.y + value, 0f);
    }
    
    public static Resources GetRepairCosts(TileObject tileObject, Building building) {
        float costFactor = 1 - building.currentHealth / building.maxHealth;
        return new Resources {
            wood = (int) Mathf.Floor(tileObject.blueprint.resources.wood * costFactor),
            waste = (int) Mathf.Floor(tileObject.blueprint.resources.waste * costFactor),
            whiskey = (int) Mathf.Floor(tileObject.blueprint.resources.whiskey * costFactor)
        };
    }
    
    public void ReplaceObject(TileObject newObject, bool showAnimation = true) {
        
        if (tileObject) {
            
            if (type == TileObjectType.Building) {
                playerManager.RemoveBuilding(tileObjectBuilding);
            }
            Destroy(tileObject.gameObject);
        }

        if (showAnimation) {
            StartEffect();
        }
        GameObject tileGameObject = Instantiate(newObject.blueprint.prefab, spawnPoint.transform.position, objectRotation, transform);
        InitializeTile(tileGameObject);

        if (type == TileObjectType.Building) {
            tileObjectBuilding = tileObject.gameObject.AddComponent<Building>();
            tileObjectBuilding.maxHealth = newObject.blueprint.buildingMaxHealth;
            tileObjectBuilding.upgrade = newObject.blueprint.buildingUpgradePrefab;
            playerManager.AddBuilding(tileObjectBuilding);
        }
        tileObject.parentTile = this;
    }
    
    private void InitializeTile(GameObject newObject) {
        tileObject = newObject.GetComponent<TileObject>();
        type = tileObject.blueprint.type;
        SetResources(tileObject.blueprint.resources);
        objectRotation = spawnPoint.transform.rotation;
    }
    
    private void StartEffect() {
        GameObject effectInstance = Instantiate(replaceEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 1f);
    }
    
    #endregion
    
    
    #region Resource Management Methods
    
    public Resources GetPlayerResources() => playerManager.resources;
    
    public void AddResources(Resources resourcesToAdd) {
        resources = new Resources {
            wood = resourcesToAdd.wood,
            waste = resourcesToAdd.waste,
            whiskey = resourcesToAdd.whiskey
        };
    }
    
    private void SetResources(Resources resourcesToSet) {
        resources = new Resources {
            wood = resourcesToSet.wood,
            waste = resourcesToSet.waste,
            whiskey = resourcesToSet.whiskey
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
    
    private void CacheManagers() {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        menuManager = FindObjectOfType<MenuManager>();
        playerManager = FindObjectOfType<PlayerManager>();
    }
    
    #endregion
}