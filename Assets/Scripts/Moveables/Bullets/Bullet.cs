using UnityEngine;

public class Bullet : MonoBehaviour {
    
    [Header("Bullet")]
    public float speed;
    public int damage;
    public float attackHeight;
    public GameObject impactEffect;
    
    protected GameObject Target;
    
    private Vector3 _targetPosition;

    #region Unity methods
    
    private void Start() {
        Vector3 direction = _targetPosition - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    
    #endregion
    
    #region Public methods

    public void Seek(GameObject turretTarget) {
        Target = turretTarget;
        _targetPosition = Target.transform.position;
    }
    
    #endregion
    
    #region Class methods
    
    protected void Damage(Enemy enemy) {
        enemy.SetHealth(enemy.GetHealth() - damage);
    }
    
    protected void StartImpactEffect() {
        GameObject effectInst = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInst, 1f);
        Destroy(gameObject);
    }

    protected Vector3 GetTargetDirection() {
        _targetPosition = Target.transform.position;
        float targetHeight = (Target.GetComponent<CapsuleCollider>().height * attackHeight) + _targetPosition.y;
        Vector3 newPos = new Vector3(_targetPosition.x, targetHeight, _targetPosition.z);
        return newPos - transform.position;
    }
    
    #endregion
}