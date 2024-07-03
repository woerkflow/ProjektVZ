using UnityEngine;

public class UIMenu : MonoBehaviour {
    
    
    #region Unity methods

    private void Start() {
        gameObject.SetActive(false);
    }
    
    #endregion
    
    
    #region Public class methods

    public void Activate() {
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }
    
    #endregion
}