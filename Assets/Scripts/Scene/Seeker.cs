using System.Collections.Generic;
using UnityEngine;

public class Seeker : MonoBehaviour {
    
    [HideInInspector]
    public List<Turret> turrets;
    [HideInInspector]
    public List<Spawn> spawns;
    [HideInInspector]
    public List<Mine> mines;
    
    private EnemySpawner _enemySpawner;
    
    
    #region Unity methods

    private void Start() {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
    }

    #endregion
    
    
    #region Public class methods

    public void RegisterTurret(Turret turret) {
        turrets.Add(turret);
    }
    
    public void RegisterSpawn(Spawn spawn) {
        spawns.Add(spawn);
    }
    
    public void RegisterMine(Mine mine) {
        mines.Add(mine);
    }

    public void UnregisterTurret(Turret turret) {
        turrets.Remove(turret);
    }
    
    public void UnregisterSpawn(Spawn spawn) {
        spawns.Remove(spawn);
    }
    
    public void UnregisterMine(Mine mine) {
        mines.Remove(mine);
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void UpdateTarget() {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        
        foreach (SwarmManager swarmManager in _enemySpawner.swarmManagers) {
            foreach (Enemy enemy in swarmManager.zombies) {
                if (!enemy.gameObject.activeSelf) {
                    continue;
                }
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

                if (distanceToEnemy < shortestDistance) {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy.gameObject;
                }
            }
        }

        foreach (Turret turret in turrets) {
            turret.SetTarget(nearestEnemy);
        }
        
        foreach (Spawn spawn in spawns) {
            spawn.SetTarget(nearestEnemy);
        }
        
        foreach (Mine mine in mines) {
            mine.SetTarget(nearestEnemy);
        }
    }
    
    #endregion
}