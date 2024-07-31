using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawn : Seeker {
    
    [Header("Spawn")] 
    public Type type;
    
    public enum Type {
        Chicken,
        Bull
    }

    private GameObject _parentSpawner;
    
    [Header("Movement")] 
    public float speed;
    
    private Vector3[] _points;
    private Vector3 _direction;
    
    [Header("Explosion")]
    public CapsuleCollider capsuleCollider;
    public int damage;
    public GameObject impactEffect;
    public Explosive explosive;
    
    private CapsuleCollider _targetCollider;
    
    
    #region Unity methods
    
    private void Start() {
        
        // Set direction points
        _points = new [] {
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
        
        // Initialize direction
        _direction = Vector3.zero;
        
        // Calculate maxColliders by perception range
        HitColliders = new Collider[GetMaxColliders(perceptionRange)];
        
        // Start coroutines
        InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
        InvokeRepeating(nameof(UpdateDirection), 0f, 1f);
    }
    
    private void Update() {
        
        if (_direction == Vector3.zero) {
            return;
        }
        RotateToTarget(_direction);
        MoveToTarget(_direction);
    }
    
    #endregion
    
    
    #region Public class methods

    public void SetParent(GameObject parent) {
        _parentSpawner = parent;
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void UpdateDirection() {
        
        if (Target == null) {
            Vector3 newRandomPosition = new Vector3(transform.position.x, 0f, transform.position.z) + _points[Random.Range(0, _points.Length)] * 0.001f;
            _direction = newRandomPosition - new Vector3(transform.position.x, 0f, transform.position.z);
            _targetCollider = null;
            return;
        }
            
        if (_targetCollider == null) {
            _targetCollider = Target.GetComponent<CapsuleCollider>();
        }
        _direction = new Vector3(Target.transform.position.x, 0f, Target.transform.position.z) - new Vector3(transform.position.x, 0f, transform.position.z);
    
        if (_direction.magnitude > capsuleCollider.radius + _targetCollider.radius) {
            return;
        }

        switch (type) {
            case Type.Chicken:
                explosive.Explode(damage, impactEffect);
                Destroy(gameObject);
                break;
            case Type.Bull:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private static float DeltaSpeed(float value) => value * Time.deltaTime;
    
    private void MoveToTarget(Vector3 direction) {
        transform.Translate(direction.normalized * DeltaSpeed(speed), Space.World);
    }
    
    private void RotateToTarget(Vector3 direction) {
        transform.rotation = Quaternion.LookRotation(direction);
    }
    
    #endregion
}
