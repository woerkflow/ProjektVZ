using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour {
    
    public static UpgradeManager Instance;
    
    [Header("Menu")]
    public UIMenu upgradeMenu;
    public TMP_Text buildingHealth;
    
    // Manager
    private PlayerManager _playerManager;
    private BuildManager _buildManager;
    private Tile _selectedTile;
    private Building _tileBuilding;
    
    // Upgrade
    private int _woodCost;
    private int _wasteCost;
    private int _whiskeyCost;
    
    #region Unity methods
    
    private void Awake() {

        if (Instance != null) {
            Debug.Log("More than one UpgradeManager at once;");
        } else {
            Instance = this;
        }
    }
    
    #endregion
    
    
    #region Private class methods

    private bool CanRepair() {
        
        if (_tileBuilding.GetHealth() < _tileBuilding.maxHealth) {
            float costFactor = 1 - (_tileBuilding.GetHealth() / _tileBuilding.maxHealth);
            _woodCost = (int) Mathf.Floor(_selectedTile.tileObject.blueprint.resourceWood * costFactor);
            _wasteCost = (int) Mathf.Floor(_selectedTile.tileObject.blueprint.resourceWaste * costFactor);
            _whiskeyCost = (int) Mathf.Floor(_selectedTile.tileObject.blueprint.resourceWhiskey * costFactor);

            return _playerManager.GetResourceWood() >= _woodCost
                   && _playerManager.GetResourceWaste() >= _wasteCost
                   && _playerManager.GetResourceWhiskey() >= _whiskeyCost;
        }
        return false;
    }
    
    #endregion
    
    
    #region Public class methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
        _tileBuilding = _selectedTile.tileObject.gameObject.GetComponent<Building>();
        upgradeMenu.Activate();
    }
    
    public void ActivateMenu() {
        buildingHealth.SetText(_tileBuilding.GetHealth() + "/" + _tileBuilding.maxHealth);
        upgradeMenu.Activate();
    }
    
    public bool MenuIsActive() {
        return upgradeMenu.IsActive();
    }
    
    #endregion
    
    
    #region Menu methods

    public void Upgrade() {

        if (_tileBuilding.upgrade) {
            _buildManager.SelectTile(_selectedTile);
            _buildManager.SelectBuildingToBuild(_tileBuilding.upgrade);
            _buildManager.Build();
            CloseMenu();
        }
    }

    public void Repair() {
        
        if (CanRepair()) {
            _playerManager.SetResourceWood(_playerManager.GetResourceWood() - _woodCost);
            _playerManager.SetResourceWaste(_playerManager.GetResourceWaste() - _wasteCost);
            _playerManager.SetResourceWhiskey(_playerManager.GetResourceWhiskey() - _whiskeyCost);
            _tileBuilding.SetHealth(_tileBuilding.maxHealth);
            CloseMenu();
        }
    }

    public void Destroy() {
        _tileBuilding.Destroy();
        CloseMenu();
    }
    
    public void CloseMenu() {
        upgradeMenu.Deactivate();
        _selectedTile = null;
        _tileBuilding = null;
    }
    
    #endregion
}
