using System.Collections;
using UnityEngine;

public class DamageArea : MonoBehaviour {
    
    [SerializeField] private DamageAreaBlueprint blueprint;

    private Coroutine _tickCoroutine;

    
    #region Unity Methods

    private void Start() {
        _tickCoroutine = StartCoroutine(
            StartTick(
                blueprint.duration, 
                blueprint.tickTime, 
                blueprint.explosive, 
                blueprint.minDamage, 
                blueprint.maxDamage
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