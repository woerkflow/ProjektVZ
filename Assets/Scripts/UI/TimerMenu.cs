using TMPro;
using UnityEngine;

public class TimerMenu : UIMenu {
    
    public TMP_Text roundCountText;
    public TMP_Text timerValueText;
    public TMP_Text enemyAmountText;
    
    
    #region Public class methods
    
    public void ActivateTimerMenu(Transform currentSpawnPoint, int currentRoundCount, int currentEnemyAmount) {
        SetTimerPosition(
            new Vector3(
                currentSpawnPoint.position.x, 
                transform.position.y, 
                currentSpawnPoint.position.z
            )
        );
        SetStringValue(roundCountText, "Round: " + currentRoundCount);
        SetStringValue(enemyAmountText, "Enemies: " + currentEnemyAmount);
        gameObject.SetActive(true);
    }

    public void RefreshTimer(float timerValue) {
        SetStringValue(timerValueText, System.TimeSpan.FromSeconds(timerValue).ToString("mm':'ss"));
    }
    
    public void DeactivateTimer() {
        SetTimerPosition(
            new Vector3(0f, transform.position.y, 0f)
        );
        gameObject.SetActive(false);
    }
    
    #endregion
    
    
    #region Private Methods

    private void SetTimerPosition(Vector3 newPosition) {
        transform.position = newPosition;
    }
    
    #endregion
}