using System.Collections;
using UnityEngine;

public class DamageArea : MonoBehaviour {

    [Header("Tick Explosion")]
    [SerializeField] private int duration;
    [SerializeField] private float tickTime;
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private Explosive explosive;

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
        
        if (_tickCoroutine == null) {
            return;
        }
        StopCoroutine(_tickCoroutine);
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
            
            if (!explosive) {
                yield break;
            }
            explosive.Explode(minDamage, maxDamage);
            yield return new WaitForSeconds(tickTime);
        }
        Destroy(explosive.gameObject);
    }
    
    #endregion
}