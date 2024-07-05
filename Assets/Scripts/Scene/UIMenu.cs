using UnityEngine;

public class UIMenu : MonoBehaviour {
    
    #region Public class methods

    public void Activate() {
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }
    
    #endregion
}