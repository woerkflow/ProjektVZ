using UnityEngine;

public class Launcher : Turret {
    
    [Header("Bullet")]
    public GameObject prefabBullet;
    
    
    #region Unity methods
    
    void Update() {
        
        if (Target == null) {
            return;
        }
        LockOnTarget();
        Shoot();
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void Shoot() {
        
        if (FireCountDown <= 0f) {
            GameObject bulletGo = Instantiate(prefabBullet, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletGo.GetComponent<Bullet>();

            if (bullet != null) {
                bullet.Seek(Target);
            }
            FireCountDown = fireRate;
        } else {
            FireCountDown -= Time.deltaTime;
        }
    }
    
    #endregion
}
