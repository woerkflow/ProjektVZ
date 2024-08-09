using UnityEngine;

public interface ITargetable {
    
    GameObject GetTarget();
    void SetTarget(GameObject target);
}