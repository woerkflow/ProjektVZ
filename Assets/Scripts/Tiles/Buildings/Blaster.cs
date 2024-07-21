using UnityEngine;

public class Blaster : Turret {
    
    [Header("Laser")] 
    public GameObject prefabLaser;
    
    
    #region Unity methods
    
    private void Update() {
        
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
            GameObject laserObj = Instantiate(prefabLaser, firePoint.position, firePoint.rotation);
            Laser laser = laserObj.GetComponent<Laser>();

            if (laser != null) {
                laser.Seek(firePoint.transform, Target);
            }
            FireCountDown = fireRate;
        } else {
            FireCountDown -= Time.deltaTime;
        }
    }
    
    #endregion
}
