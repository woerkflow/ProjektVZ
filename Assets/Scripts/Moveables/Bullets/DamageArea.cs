using System.Collections;
using UnityEngine;

public class DamageArea : MonoBehaviour {

    [Header("Tick Explosion")]
    public int duration;
    public float tickTime;
    public int damage;
    public Explosive explosive;
    
    
    #region Unity methods
    
    private void Start() {

        StartCoroutine(
            StartTick(
                duration, 
                tickTime,
                explosive, 
                damage, 
                gameObject
            )
        );
    }
    
    #endregion
    
    
    #region Tick Loop
    
    private static IEnumerator StartTick(
        int duration, 
        float tickTime, 
        Explosive explosive, 
        int damage, 
        GameObject gameObject
    ) {
        for (var i = 0; i < duration; i++) {
            explosive.Explode(damage);
            yield return new WaitForSeconds(tickTime);
        }
        Destroy(gameObject);
    }
    
    #endregion
}