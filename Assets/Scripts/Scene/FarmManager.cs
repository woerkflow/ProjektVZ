using TMPro;
using UnityEngine;

public class FarmManager : MonoBehaviour {
    
    public static FarmManager Instance;
    
    [Header("Menu")]
    public UIMenu farmMenu;
    public TMP_Text resourceWoodAmount;
    public TMP_Text resourceWasteAmount;
    public TMP_Text resourceWhiskeyAmount;
    public TMP_Text timeCosts;

    private EnemySpawner _enemySpawner;
    private PlayerManager _playerManager;
    private Tile _selectedTile;

    
    #region Unity Methods
    
    private void Awake() {
        if (Instance) {
            Debug.LogWarning("More than one FarmManager instance found!");
            return;
        }
        Instance = this;
    }

    private void Start() {
        _enemySpawner = EnemySpawner.Instance;
        _playerManager = PlayerManager.Instance;
        farmMenu.Deactivate();
    }
    
    #endregion

    
    #region Public Methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
    }

    public void ActivateMenu() {
        Resources tileResources = _selectedTile.GetResources();
        SetMenuResourceValue(resourceWoodAmount, tileResources.wood);
        SetMenuResourceValue(resourceWasteAmount, tileResources.waste);
        SetMenuResourceValue(resourceWhiskeyAmount, tileResources.whiskey);
        SetMenuResourceValue(timeCosts, _selectedTile.GetTileObject().blueprint.timeCosts);
        farmMenu.Activate();
    }
    
    public bool MenuIsActive() {
        return farmMenu.IsActive();
    }
    
    #endregion

    
    #region Private Methods

    private static bool CanFarm(EnemySpawner enemySpawner, Tile selectedTile) {
        TileObjectBlueprint tileObjectBlueprint = selectedTile.GetTileObject().blueprint;
        return enemySpawner.GetTime() >= tileObjectBlueprint.timeCosts
               && selectedTile.HasResources( new Resources {
                   wood = tileObjectBlueprint.resourceWood,
                   waste = tileObjectBlueprint.resourceWaste,
                   whiskey = tileObjectBlueprint.resourceWhiskey
               });
    }

    private static void SetMenuResourceValue(TMP_Text element, int value) {
        element.SetText(value.ToString());
    }
    
    private void DeductResourcesAndDestroyTileObject() {
        Resources tileResources = _selectedTile.GetResources();

        _playerManager.AddResources(tileResources);
        _selectedTile.ClearResources();

        _enemySpawner.SetTimer(_enemySpawner.GetTime() - _selectedTile.GetTileObject().blueprint.timeCosts);
        _selectedTile.GetTileObject().DestroyObject();
    }

    #endregion

    
    #region Menu Button Methods

    public void Farm() {
        
        if (CanFarm(_enemySpawner, _selectedTile)) {
            DeductResourcesAndDestroyTileObject();
            CloseMenu();
        }
    }
    
    public void CloseMenu() {
        farmMenu.Deactivate();
        _selectedTile = null;
    }
    
    #endregion
}
