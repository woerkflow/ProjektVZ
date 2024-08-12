using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwarmManager : MonoBehaviour {
    
    private readonly List<Enemy> _enemies = new();
    private SpawnPoint _spawnPoint;
    private bool _isDestroyed;
    
    private Coroutine _updateTargetsCoroutine;
    private PlayerManager _playerManager;
    private Building _target;

    
    #region Unity Methods
    
    private void Start() {
        _isDestroyed = false;
        _playerManager = FindObjectOfType<PlayerManager>();
        _updateTargetsCoroutine = StartCoroutine(UpdateTargetsRoutine());
    }
    
    private void OnDestroy() {
        
        if (_updateTargetsCoroutine != null) {
            StopCoroutine(_updateTargetsCoroutine);
        }
    }

    #endregion

    
    #region Zombie Subscription
    
    public void Register(Enemy enemy) {
        
        if (!_enemies.Contains(enemy)) {
            _enemies.Add(enemy);
        }
    }

    public void Unregister(Enemy enemy) {
        _enemies.Remove(enemy);
    }

    public void SetSpawnPoint(SpawnPoint spawnPoint) {
        _spawnPoint = spawnPoint;
    }
    
    #endregion

    
    #region Zombie Targeting
    
    private IEnumerator UpdateTargetsRoutine() {
        
        while (!_isDestroyed) {
            yield return new WaitForSeconds(1f);
            UpdateTarget();
        }
    }

    private void UpdateTarget() {

        if (_enemies.Count <= 0) {
            _isDestroyed = true;
            Destroy(gameObject);
            return;
        }
        _target = FindNearestBuilding();

        if (!_target) {
            return;
        }
        
        foreach (Enemy enemy in _enemies.Where(enemy => enemy.target != _target.gameObject)) {
            enemy.SetTarget(_target);
        }
    }

    private Building FindNearestBuilding() {
        List<Building> buildings = _playerManager.GetBuildings();
        float shortestDistance = Mathf.Infinity;
        Building nearestBuilding = null;

        foreach (Building building in buildings) {
            float distanceToBuilding = Vector3.Distance(_spawnPoint.transform.position, building.transform.position);

            if (!(distanceToBuilding < shortestDistance)) {
                continue;
            }
            shortestDistance = distanceToBuilding;
            nearestBuilding = building;
        }
        return nearestBuilding;
    }

    #endregion
}