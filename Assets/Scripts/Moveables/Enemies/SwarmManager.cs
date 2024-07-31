using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwarmManager : MonoBehaviour {
    
    [Header("Swarm")]
    public List<Enemy> zombies;
    public GameObject mainTarget;
    
    private BuildManager _buildManager;

    
    #region Unity Methods
    
    private void Start() {
        _buildManager = BuildManager.Instance;
        
        // Start coroutine for targeting
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
    
    #endregion
    
    
    #region Zombie Targeting
    
    private void UpdateTarget() {
        Enemy leader = zombies.FirstOrDefault();

        if (leader == null) {
            Destroy(gameObject);
            return;
        }
        GameObject leaderTarget = leader.GetTarget();
        
        if (leaderTarget && leaderTarget != mainTarget) {
            return;
        }
        List<GameObject> buildings = _buildManager.GetBuildings();
        float shortestDistance = leader.perceptionRange;
        GameObject nearestBuilding = null;
        
        foreach (GameObject building in buildings) {
            float distanceToEnemy = Vector3.Distance(leader.transform.position, building.transform.position);
        
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestBuilding = building;
            }
        }

        if (nearestBuilding == null) {
            return;
        }

        foreach (Enemy zombie in zombies) {
            zombie.SetTarget(nearestBuilding);
        }
    }
    
    #endregion
}