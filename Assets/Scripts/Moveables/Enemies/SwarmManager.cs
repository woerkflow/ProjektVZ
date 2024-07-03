using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwarmManager : MonoBehaviour {
    public List<Enemy> zombies;
    public GameObject mainTarget;
    private GameObject _target;

    #region Unity Methods
    
    private void Start() {
        _target = mainTarget;
        
        // Start coroutine for targeting
        InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
    }
    
    #endregion
    
    #region Zombie Subscription
    
    public void Join(Enemy zombie) {
        zombie.SetTarget(_target);
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
        
        if (_target && _target != mainTarget) {
            return;
        }
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(leader.transform.position, leader.perceptionRange, hitColliders);
        float shortestDistance = leader.perceptionRange;
        GameObject nearestEnemy = null;
        
        for (int i = 0; i < numColliders; i++) {
            Collider enemy = hitColliders[i];
            
            if (enemy.CompareTag(leader.enemyTag)) {
                float distanceToEnemy = Vector3.Distance(leader.transform.position, enemy.transform.position);
            
                if (distanceToEnemy < shortestDistance) {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy.gameObject;
                }
            }
        }
        _target = nearestEnemy;

        foreach (Enemy zombie in zombies) {
            zombie.SetTarget(_target);
        }
    }
    
    #endregion
}