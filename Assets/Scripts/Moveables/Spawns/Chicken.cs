using UnityEngine;

public class Chicken : Spawn {
    
    [Header("Explosion")]
    public CapsuleCollider capsuleCollider;
    public int damage;
    public GameObject impactEffect;
    public Explosive explosive;

    private CapsuleCollider _targetCollider;

    private void Start() {
        Points = new [] {
            new Vector3(-1,0,-1), 
            Vector3.left,
            new Vector3(-1,0,1), 
            Vector3.back,
            Vector3.zero, 
            Vector3.forward,
            new Vector3(1,0,-1), 
            Vector3.right,
            new Vector3(1,0,1),
        };
        Direction = Vector3.zero;
        InvokeRepeating(nameof(UpdateDirection), 0f, 3f);
    }

    private void UpdateDirection() {
        
        if (Target == null) {
            Vector3 newRandomPosition = new Vector3(transform.position.x, 0f, transform.position.z) + Points[Random.Range(0, Points.Length)] * 0.001f;
            Direction = newRandomPosition - new Vector3(transform.position.x, 0f, transform.position.z);
            _targetCollider = null;
            return;
        }
            
        if (_targetCollider == null) {
            _targetCollider = Target.GetComponent<CapsuleCollider>();
        }
        Direction = Target.transform.position - new Vector3(transform.position.x, 0f, transform.position.z);
    
        if (Direction.magnitude > capsuleCollider.radius + _targetCollider.radius) {
            return;
        }
        explosive.Explode(damage, impactEffect);
        Destroy(gameObject);
    }
    
    void Update() {

        if (Direction != Vector3.zero) {
            RotateToTarget(Direction);
            MoveToTarget(Direction);
        }
    }
}
