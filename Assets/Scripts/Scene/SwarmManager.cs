using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwarmManager : MonoBehaviour {
    
    private List<Enemy> _enemies = new();
    private BuildManager _buildManager;
    private SpawnPoint _spawnPoint;
    private GameObject _target;
    private bool _isDestroyed;
    private Coroutine _updateTargetsCoroutine;

    
    #region Unity Methods
    
    private void Start() {
        _isDestroyed = false;
        _buildManager = BuildManager.Instance;
        _updateTargetsCoroutine = StartCoroutine(UpdateTargetRoutine());
    }
    
    private void OnDestroy() {
        
        if (_updateTargetsCoroutine != null) {
            StopCoroutine(_updateTargetsCoroutine);
        }
    }

    #endregion

    
    #region Zombie Subscription

    public List<Enemy> GetEnemies() => _enemies;
    
    public void Join(Enemy enemy) {
        
        if (!_enemies.Contains(enemy)) {
            _enemies.Add(enemy);
        }
    }

    public void Leave(Enemy enemy) {
        _enemies.Remove(enemy);
    }

    public void SetSpawnPoint(SpawnPoint spawnPoint) {
        _spawnPoint = spawnPoint;
    }
    
    #endregion

    
    #region Zombie Targeting
    
    private IEnumerator UpdateTargetRoutine() {
        
        while (!_isDestroyed) {
            UpdateTarget();
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateTarget() {
        _enemies.RemoveAll(enemy => enemy == null);

        if (_enemies.Count <= 0) {
            _isDestroyed = true;
            Destroy(gameObject);
            return;
        }
        _target = FindNearestBuilding();

        if (_target == null) {
            return;
        }
        
        foreach (Enemy enemy in _enemies.Where(enemy => enemy.target != _target)) {
            enemy.SetTarget(_target);
        }
    }

    private GameObject FindNearestBuilding() {
        List<GameObject> buildings = _buildManager.GetBuildings();
        float shortestDistance = Mathf.Infinity;
        GameObject nearestBuilding = null;

        foreach (GameObject building in buildings) {
            float distanceToBuilding = Vector3.Distance(_spawnPoint.transform.position, building.transform.position);

            if (distanceToBuilding < shortestDistance) {
                shortestDistance = distanceToBuilding;
                nearestBuilding = building;
            }
        }
        return nearestBuilding;
    }

    #endregion
}