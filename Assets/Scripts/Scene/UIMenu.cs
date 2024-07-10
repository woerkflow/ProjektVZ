using UnityEngine;

public class UIMenu : MonoBehaviour {
    
    public Camera userCamera;
    
    #region Unity Methods

    private void Update() {
        RotateMenuToCamera();
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
    
    #region Privat class mehtods
    
    private void RotateMenuToCamera() {
        Vector3 direction = transform.position - userCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
    }
    
    #endregion
}