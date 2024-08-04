using UnityEngine;

public class UIMenu : MonoBehaviour {
    
    public Camera userCamera;
    
    
    #region Unity Methods

    private void Update() {
        transform.rotation = RotateMenuToCamera(transform, userCamera);
    }

    #endregion
    
    
    #region Public class methods

    public void Activate() {

        if (!IsActive()) {
            gameObject.SetActive(true);
        }
    }

    public void Deactivate() {

        if (IsActive()) {
            gameObject.SetActive(false);
        }
    }

    public bool IsActive() {
        return gameObject.activeSelf;
    }
    
    #endregion
    
    
    #region Private class methods
    
    private static Quaternion RotateMenuToCamera(Transform transform, Camera userCamera) {
        Vector3 direction = transform.position - userCamera.transform.position;
        return Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
    }
    
    #endregion
}