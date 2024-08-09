using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    
    public static PlayerManager Instance;
    
    [Header("UI")]
    public TMP_Text resourceWoodText;
    public TMP_Text resourceWasteText;
    public TMP_Text resourceWhiskeyText;
    
    private Resources _resources;

    
    #region Unity Methods
    
    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("More than one PlayerManager instance found!");
            return;
        }
        Instance = this;
    }

    private void Start() {
        _resources = new Resources {
            wood = 100,
            waste = 100, 
            whiskey = 100
        };
        UpdateResourceUI();
    }

    #endregion

    
    #region Public Methods

    public void AddResources(Resources resourcesToAdd) {
        _resources.wood += resourcesToAdd.wood;
        _resources.waste += resourcesToAdd.waste;
        _resources.whiskey += resourcesToAdd.whiskey;
        UpdateResourceUI();
    }

    public void SubtractResources(Resources resourcesToSubtract) {
        _resources.wood -= resourcesToSubtract.wood;
        _resources.waste -= resourcesToSubtract.waste;
        _resources.whiskey -= resourcesToSubtract.whiskey;
        UpdateResourceUI();
    }

    public bool HasEnoughResources(Resources requiredResources) {
        return _resources.wood >= requiredResources.wood &&
               _resources.waste >= requiredResources.waste &&
               _resources.whiskey >= requiredResources.whiskey;
    }

    #endregion

    
    #region Private Methods

    private void UpdateResourceUI() {
        SetMenuResourceValue(resourceWoodText, _resources.wood);
        SetMenuResourceValue(resourceWasteText, _resources.waste);
        SetMenuResourceValue(resourceWhiskeyText, _resources.whiskey);
    }

    private static void SetMenuResourceValue(TMP_Text element, int value) {
        element.SetText(value.ToString());
    }
    
    #endregion
}