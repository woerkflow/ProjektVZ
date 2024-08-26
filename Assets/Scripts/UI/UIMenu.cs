using TMPro;
using UnityEngine;

public class UIMenu : MonoBehaviour {
    
    [SerializeField] private Camera userCamera;
    
    
    #region Unity Methods

    private void Update() {
        transform.rotation = RotateMenuToCamera(transform, userCamera);
    }

    #endregion
    
    
    #region Public Class Methods

    public void Activate() {

        if (IsActive()) {
            return;
        }
        gameObject.SetActive(true);
    }

    public void Deactivate() {

        if (!IsActive()) {
            return;
        }
        gameObject.SetActive(false);
    }

    public bool IsActive() {
        return gameObject.activeSelf;
    }
    
    protected static void SetStringValue(TMP_Text element, string value) {
        element.SetText(value);
    }
    
    protected static void SetIntValue(TMP_Text element, int value) {
        element.SetText(value.ToString());
    }
    
    public void SetPosition(Tile tile, float height) {
        Vector3 tileSpawnPointPosition = tile.GetSpawnPoint().position;
        transform.position =
            new Vector3(
                tileSpawnPointPosition.x,
                tileSpawnPointPosition.y + height,
                tileSpawnPointPosition.z
            );
    }
    
    #endregion
    
    
    #region Private Class Methods
    
    private static Quaternion RotateMenuToCamera(Transform transform, Camera userCamera) {
        Vector3 direction = transform.position - userCamera.transform.position;
        return Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
    }
    
    #endregion
}