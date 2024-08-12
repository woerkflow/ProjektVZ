using TMPro;
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
    
    public static void SetStringValue(TMP_Text element, string value) {
        element.SetText(value);
    }
    
    public static void SetIntValue(TMP_Text element, int value) {
        element.SetText(value.ToString());
    }
    
    #endregion
    
    
    #region Private class methods
    
    private static Quaternion RotateMenuToCamera(Transform transform, Camera userCamera) {
        Vector3 direction = transform.position - userCamera.transform.position;
        return Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
    }
    
    #endregion
}