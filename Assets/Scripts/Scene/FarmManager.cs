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
    
    #region Unity methods
    
    private void Awake() {

        if (Instance != null) {
            Debug.Log("More than one FarmManager at once;");
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
    }

    public void ActivateMenu() {
        SetMenuResourceValue(resourceWoodAmount, _selectedTile.resourceWood);
        SetMenuResourceValue(resourceWasteAmount, _selectedTile.resourceWaste);
        SetMenuResourceValue(resourceWhiskeyAmount, _selectedTile.resourceWhiskey);
        SetMenuResourceValue(timeCosts, _selectedTile.GetTileObject().blueprint.timeCosts);
        farmMenu.Activate();
    }
    
    public bool MenuIsActive() {
        return farmMenu.IsActive();
    }
    
    #endregion
    
    #region Private class methods

    private bool CanFarm() {
        
        return _enemySpawner.GetTime() >= _selectedTile.GetTileObject().blueprint.timeCosts
               && (_selectedTile.resourceWood > 0
               || _selectedTile.resourceWaste > 0
               || _selectedTile.resourceWhiskey > 0);
    }
    
    private void SetMenuResourceValue(TMP_Text element, int value) {
        element.SetText(value.ToString());
    }
    
    #endregion
    
    #region Menu button methods

    public void Farm() {

        if (CanFarm()) {
            
            if (_selectedTile.resourceWood > 0) {
                _playerManager.SetResourceWood(
                    _playerManager.GetResourceWood() + _selectedTile.resourceWood
                );
                _selectedTile.resourceWood = 0;
            }
            
            if (_selectedTile.resourceWaste > 0) {
                _playerManager.SetResourceWaste(
                    _playerManager.GetResourceWaste() + _selectedTile.resourceWaste
                );
                _selectedTile.resourceWaste = 0;
            }
            
            if (_selectedTile.resourceWhiskey > 0) {
                _playerManager.SetResourceWhiskey(
                    _playerManager.GetResourceWhiskey() + _selectedTile.resourceWhiskey
                );
                _selectedTile.resourceWhiskey = 0;
            }
            _enemySpawner.SetTimer(
                _enemySpawner.GetTime() - _selectedTile.GetTileObject().blueprint.timeCosts
            );
            _selectedTile.GetTileObject().DestroyObject();
            CloseMenu();
        }
    }
    
    public void CloseMenu() {
        farmMenu.Deactivate();
        _selectedTile = null;
    }
    
    #endregion
}
