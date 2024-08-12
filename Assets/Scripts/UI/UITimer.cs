using TMPro;
using UnityEngine;

public class UITimer : UIMenu {
    
    public TMP_Text timerText;
    public TMP_Text amountText;
    
    
    #region Public class methods
    
    public void ActivateTimer(Transform currentSpawnPoint, int currentEnemyAmount) {
        transform.position = new Vector3(
            currentSpawnPoint.position.x,
            transform.position.y,
            currentSpawnPoint.position.z
        );
        SetStringValue(amountText, currentEnemyAmount.ToString());
        gameObject.SetActive(true);
    }

    public void RefreshTimer(float buildCountDown) {
        SetStringValue(timerText, System.TimeSpan.FromSeconds(buildCountDown).ToString("mm':'ss"));
    }
    
    public void DeactivateTimer() {
        transform.position = new Vector3(
            0f,
            transform.position.y,
            0f
        );
        gameObject.SetActive(false);
    }
    
    #endregion
}