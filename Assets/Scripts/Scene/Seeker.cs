using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Seeker : MonoBehaviour {
    
    private List<ITargetable> _targetables = new();
    private EnemySpawner _enemySpawner;
    private Coroutine _updateTargetsCoroutine;
    private bool _isDestroyed;

    
    #region Unity Methods

    private void Start() {
        InitializeManagers();
        
        if (_enemySpawner != null) {
            _isDestroyed = false;
            _updateTargetsCoroutine = StartCoroutine(UpdateTargetsCoroutine());
        } else {
            Debug.LogError("EnemySpawner not found in the scene.");
        }
    }

    private void OnDestroy() {
        _isDestroyed = true;
        
        if (_updateTargetsCoroutine != null) {
            StopCoroutine(_updateTargetsCoroutine);
        }
    }

    #endregion

    
    #region Public Methods

    public void Register(ITargetable targetable) {
        
        if (!_targetables.Contains(targetable)) {
            _targetables.Add(targetable);
        }
    }

    public void Unregister(ITargetable targetable) {
        _targetables.Remove(targetable);
    }

    #endregion

    
    #region Private Methods

    private void InitializeManagers() {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    private IEnumerator UpdateTargetsCoroutine() {
        
        while (!_isDestroyed) {
            GameObject nearestEnemy = FindNearestEnemy();

            foreach (ITargetable targetable in _targetables.Where(targetable => targetable.GetTarget() != nearestEnemy)) {
                targetable.SetTarget(nearestEnemy);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private GameObject FindNearestEnemy() {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (SwarmManager swarmManager in _enemySpawner.swarmManagers) {
            foreach (Enemy enemy in swarmManager.GetEnemies().Where(enemy => enemy.gameObject.activeSelf)) {
                
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                
                if (distanceToEnemy < shortestDistance) {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy.gameObject;
                }
            }
        }
        return nearestEnemy;
    }

    #endregion
}