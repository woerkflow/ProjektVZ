using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    
    public static PlayerManager Instance;
    
    [Header("UI")]
    public TMP_Text resourceWoodText;
    public TMP_Text resourceWasteText;
    public TMP_Text resourceWhiskeyText;
    
    // Resources
    private int _resourceWood;
    private int _resourceWaste;
    private int _resourceWhiskey;
    
    
    #region Unity methods
    
    private void Awake() {

        if (Instance != null) {
            Debug.Log("More than one PlayerManager at once;");
        } else {
            Instance = this;
        }
    }

    private void Start() {
        
        // Give start resources to the player
        SetResourceWood(100);
        SetResourceWaste(100);
        SetResourceWhiskey(100);
    }

    #endregion
    
    #region Public class methods

    public int GetResourceWood() {
        return _resourceWood;
    }

    public void SetResourceWood(int value) {
        _resourceWood = value;
        SetMenuResourceValue(resourceWoodText, _resourceWood);
    }
    
    public int GetResourceWaste() {
        return _resourceWaste;
    }

    public void SetResourceWaste(int value) {
        _resourceWaste = value;
        SetMenuResourceValue(resourceWasteText, _resourceWaste);
    }
    
    public int GetResourceWhiskey() {
        return _resourceWhiskey;
    }

    public void SetResourceWhiskey(int value) {
        _resourceWhiskey = value;
        SetMenuResourceValue(resourceWhiskeyText, _resourceWhiskey);
    }
    
    #endregion
    
    #region Private class methods

    private void SetMenuResourceValue(TMP_Text element, int value) {
        element.SetText(value.ToString());
    }
    
    #endregion
}