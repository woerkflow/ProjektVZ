using System;
using UnityEngine;

public class Seeker : MonoBehaviour {
    
    [Header("Target")]
    public string enemyTag;
    public float perceptionRange;
    public Type type;
    public Turret turret;
    public Spawn spawn;
    public Mine mine;

    public enum Type {
        Turret,
        Spawn,
        Mine
    }
    
    private GameObject _target;
    private Collider[] _hitColliders;
    
    #region Unity methods

    private void Start() {
        
        // Calculate maxColliders by perception range
        _hitColliders = new Collider[GetMaxColliders(perceptionRange)];
        
        // Start coroutines
        InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
    }

    #endregion
    
    
    #region Private class methods
    
    private void UpdateTarget() {

        if (_target != null 
            && _target.activeSelf
            && _target.CompareTag(enemyTag)) {
            return;
        }
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, perceptionRange, _hitColliders);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        for (int i = 0; i < enemyCount; i++) {
            Collider enemy = _hitColliders[i];

            if (enemy.gameObject.activeSelf && enemy.CompareTag(enemyTag)) {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

                if (distanceToEnemy < shortestDistance) {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy.gameObject;
                }
            }
        }
        SetTarget(nearestEnemy);
    }

    private void SetTarget(GameObject target) {
        _target = target;

        if (_target == null || !_target.activeSelf) {
            _target = null;
            return;
        }

        switch (type) {
            case Type.Turret:
                turret.SetTarget(target);
                break;
            case Type.Spawn:
                spawn.SetTarget(target);
                break;
            case Type.Mine:
                mine.SetTarget(target);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static int GetMaxColliders(float range) {
        float diameterInTiles = range * 50;
        float quadInTiles = Mathf.Pow(diameterInTiles, 2f);
        return (int)(quadInTiles + 5);
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, perceptionRange);
    }
    
    #endregion
}