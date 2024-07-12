using TMPro;
using UnityEngine;

public class FarmManager : MonoBehaviour {
    
    public static FarmManager Instance;

    [Header("Tile")] 
    public TileObject empty;

    [Header("Timer")] 
    public UITimer timer;
    
    [Header("Menu")]
    public UIMenu farmMenu;
    public TMP_Text resourceWoodAmount;
    public TMP_Text resourceWasteAmount;
    public TMP_Text resourceWhiskeyAmount;
    public TMP_Text timeCosts;

    private EnemySpawner _enemySpawner;
    private PlayerManager _playerManager;
    private Tile _selectedTile;
    
    #region Unity methods
    
    private void Awake() {

        if (Instance != null) {
            Debug.Log("More than one BuildManager at once;");
        } else {
            Instance = this;
        }
    }

    private void Start() {
        _enemySpawner = EnemySpawner.Instance;
        _playerManager = PlayerManager.Instance;
        farmMenu.Deactivate();
    }
    
    #endregion
    
    #region Public class methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
        SetMenuResourceValue(resourceWoodAmount, _selectedTile.ResourceWood);
        SetMenuResourceValue(resourceWasteAmount, _selectedTile.ResourceWaste);
        SetMenuResourceValue(resourceWhiskeyAmount, _selectedTile.ResourceWhiskey);
        SetMenuResourceValue(timeCosts, _selectedTile.tileObject.blueprint.timeCosts);
        farmMenu.Activate();
    }
    
    #endregion
    
    #region Private class methods

    private bool CanFarm() {
        
        return _enemySpawner.GetTime() > _selectedTile.tileObject.blueprint.timeCosts
               && (_selectedTile.ResourceWood > 0
               || _selectedTile.ResourceWaste > 0
               || _selectedTile.ResourceWhiskey > 0);
    }
    
    private void SetMenuResourceValue(TMP_Text element, int value) {
        element.SetText(value.ToString());
    }
    
    #endregion
    
    #region Menu Methods

    public void Farm() {

        if (CanFarm()) {
            
            if (_selectedTile.ResourceWood > 0) {
                _playerManager.SetResourceWood(
                    _playerManager.GetResourceWood() + _selectedTile.ResourceWood
                );
                _selectedTile.ResourceWood = 0;
            }
            
            if (_selectedTile.ResourceWaste > 0) {
                _playerManager.SetResourceWaste(
                    _playerManager.GetResourceWaste() + _selectedTile.ResourceWaste
                );
                _selectedTile.ResourceWaste = 0;
            }
            
            if (_selectedTile.ResourceWhiskey > 0) {
                _playerManager.SetResourceWhiskey(
                    _playerManager.GetResourceWhiskey() + _selectedTile.ResourceWhiskey
                );
                _selectedTile.ResourceWhiskey = 0;
            }
            _enemySpawner.SetTimer(
                _enemySpawner.GetTime() - _selectedTile.tileObject.blueprint.timeCosts
            );
            _selectedTile.ReplaceObject(empty);
            Close();
        }
    }
    
    public void Close() {
        farmMenu.Deactivate();
        _selectedTile = null;
    }
    
    #endregion
}
