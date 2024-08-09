using System.Collections;
using UnityEngine;

public class DamageArea : MonoBehaviour {

    [Header("Tick Explosion")]
    public int duration;
    public float tickTime;
    public int minDamage;
    public int maxDamage;
    public Explosive explosive;

    private Coroutine _tickCoroutine;

    
    #region Unity Methods

    private void Start() {
        _tickCoroutine = StartCoroutine(
            StartTick(
                duration, 
                tickTime, 
                explosive, 
                minDamage, 
                maxDamage
            )
        );
    }
    
    private void OnDestroy() {
        
        if (_tickCoroutine != null) {
            StopCoroutine(_tickCoroutine);
        }
    }

    #endregion

    
    #region Tick Loop

    private static IEnumerator StartTick(
        int duration,
        float tickTime,
        Explosive explosive,
        int minDamage,
        int maxDamage
    ) {
        for (int i = 0; i < duration; i++) {
            
            if (explosive == null) {
                yield break;
            }
            explosive.Explode(minDamage, maxDamage);
            yield return new WaitForSeconds(tickTime);
        }
        Destroy(explosive.gameObject);
    }

    #endregion
}