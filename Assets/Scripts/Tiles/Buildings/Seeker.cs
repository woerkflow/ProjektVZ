using System;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : MonoBehaviour {
    
    [Header("Type")]
    public Type type;
    public Turret turret;
    public Spawn spawn;
    public Mine mine;

    public enum Type {
        Turret,
        Spawn,
        Mine
    }
    
    private EnemySpawner _enemySpawner;
    private GameObject _target;
    private string _enemyTag;
    private float _perceptionRange;
    
    #region Unity methods

    private void Start() {
        
        switch (type) {
            case Type.Turret:
                _enemyTag = turret.enemyTag;
                _perceptionRange = turret.perceptionRange;
                break;
            case Type.Spawn:
                _enemyTag = spawn.enemyTag;
                _perceptionRange = spawn.perceptionRange;
                break;
            case Type.Mine:
                _enemyTag = spawn.enemyTag;
                _perceptionRange = spawn.perceptionRange;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        // Calculate maxColliders by perception range
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        
        // Start coroutines
        InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
    }

    #endregion
    
    
    #region Private class methods
    
    private void UpdateTarget() {

        if (_target != null 
            && _target.activeSelf
            && _target.CompareTag(_enemyTag)
            && Vector3.Distance(_target.transform.position, transform.position) < _perceptionRange
        ) {
            return;
        }
        float shortestDistance = _perceptionRange;
        GameObject nearestEnemy = null;
        
        foreach (SwarmManager swarmManager in _enemySpawner.swarmManagers) {
            foreach (Enemy enemy in swarmManager.zombies) {
                if (enemy.gameObject.activeSelf && enemy.CompareTag(_enemyTag)) {
                    float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

                    if (distanceToEnemy < shortestDistance) {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy.gameObject;
                    }
                }
            }
        }
        _target = nearestEnemy;

        switch (type) {
            case Type.Turret:
                turret.SetTarget(_target);
                break;
            case Type.Spawn:
                spawn.SetTarget(_target);
                break;
            case Type.Mine:
                mine.SetTarget(_target);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _perceptionRange);
    }
    
    #endregion
}