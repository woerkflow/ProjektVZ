using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    
    #region Button Menu Methods
    
    public void LoadGame() {
        SceneManager.LoadScene(1);
    }
    
    #endregion
}