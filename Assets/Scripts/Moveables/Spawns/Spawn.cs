using UnityEngine;

public class Spawn : Seeker {

    [Header("Movement")] 
    public float speed;
    
    protected Vector3[] Points;
    protected Vector3 Direction;
    

    #region Private class methods
    
    private float DeltaSpeed(float value) => value * Time.deltaTime;
    
    protected void MoveToTarget(Vector3 direction) {
        transform.Translate(direction.normalized * DeltaSpeed(speed), Space.World);
    }

    protected void RotateToTarget(Vector3 direction) {
        transform.rotation = Quaternion.LookRotation(direction);
    }
    
    #endregion
}
