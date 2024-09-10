using UnityEngine;

[System.Serializable]
public struct SpawnBlueprint {
    public SpawnType type;
    public float perceptionRange;
    public float speed;
    public CapsuleCollider capsuleCollider;
    public int minDamage;
    public int maxDamage;
    public GameObject impactEffectPrefab;
    public AudioClip impactEffectClip;
    public Explosive explosive;
}