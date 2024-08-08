using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwarmManager : MonoBehaviour {
    
    [Header("Swarm")]
    public List<Enemy> zombies;
    
    private BuildManager _buildManager;
    private SpawnPoint _spawnPoint;
    private GameObject _target;

    
    #region Unity Methods
    
    private void Start() {
        _buildManager = BuildManager.Instance;

        InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
    }
    
    #endregion
    
    
    #region Zombie Subscription
    
    public void Join(Enemy zombie) {
        zombies.Add(zombie);
    }

    public void Leave(Enemy zombie) {
        zombies.Remove(zombie);
    }

    public void SetSpawnPoint(SpawnPoint spawnPoint) {
        _spawnPoint = spawnPoint;
    }
    
    #endregion
    
    
    #region Zombie Targeting
    
    private void UpdateTarget() {
        
        if (zombies.Count <= 0) {
            Destroy(gameObject);
            return;
        }
        List<GameObject> buildings = _buildManager.GetBuildings();
        float shortestDistance = Mathf.Infinity;
        GameObject nearestBuilding = null;
        
        foreach (GameObject building in buildings) {
            float distanceToEnemy = Vector3.Distance(_spawnPoint.transform.position, building.transform.position);

            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestBuilding = building;
            }
        }
        _target = nearestBuilding;

        if (!nearestBuilding) {
            return;
        }
        
        foreach (var zombie in zombies.Where(zombie => zombie.target != _target)) {
            zombie.SetTarget(nearestBuilding);
        }
    }
    
    #endregion
}