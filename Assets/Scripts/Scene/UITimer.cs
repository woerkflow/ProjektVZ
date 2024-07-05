using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour {
    
    public Camera userCamera;
    public TMP_Text timerText;
    public TMP_Text amountText;
    
    #region Public class methods
    
    public void ActivateTimer(Transform currentSpawnPoint, int currentEnemyAmount) {
        
        // Translate timer to spawn point & rotate it to the user camera
        transform.position = new Vector3(
            currentSpawnPoint.position.x,
            transform.position.y,
            currentSpawnPoint.position.z
        );
        
        // Rotate timer
        RotateTimerToCamera();
        
        // Set text
        amountText.SetText(currentEnemyAmount.ToString());
        
        // Set active
        gameObject.SetActive(true);
    }

    public void RefreshTimer(float buildCountDown) {
        
        // Rotate timer
        RotateTimerToCamera();
        
        // Set text
        timerText.SetText(System.TimeSpan.FromSeconds(buildCountDown).ToString("mm':'ss"));
    }
    
    public void DeactivateTimer() {
        
        // Reset position
        transform.position = new Vector3(
            0f,
            transform.position.y,
            0f
        );
        
        // Set inactive
        gameObject.SetActive(false);
    }
    
    #endregion
    
    #region Private Class Methods

    private void RotateTimerToCamera() {
        Vector3 direction = transform.position - userCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
    }
    
    #endregion
}