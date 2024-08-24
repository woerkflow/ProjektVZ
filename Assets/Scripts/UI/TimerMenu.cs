using TMPro;
using UnityEngine;

public class TimerMenu : UIMenu {
    
    public TMP_Text roundCountText;
    public TMP_Text timerValueText;
    public TMP_Text enemyAmountText;
    
    
    #region Public class methods
    
    public void SetValues(int currentRoundCount, int currentEnemyAmount, float timerValue) {
        SetStringValue(roundCountText, "Round: " + currentRoundCount);
        SetStringValue(enemyAmountText, "Enemies: " + currentEnemyAmount);
        SetStringValue(timerValueText, System.TimeSpan.FromSeconds(timerValue).ToString("mm':'ss"));
    }

    public void Refresh(float timerValue) {
        SetStringValue(timerValueText, System.TimeSpan.FromSeconds(timerValue).ToString("mm':'ss"));
    }

    public void SetPosition(Transform currentSpawnPoint) {
        transform.position =
            new Vector3(
                currentSpawnPoint.position.x,
                transform.position.y,
                currentSpawnPoint.position.z
            );
    }
    
    #endregion
}