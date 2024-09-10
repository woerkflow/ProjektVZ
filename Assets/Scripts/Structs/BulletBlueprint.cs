using UnityEngine;

[System.Serializable]
public struct BulletBlueprint {
    public int minDamage;
    public int maxDamage;
    public float impactHeight;
    public Transform impactAnchor;
    public GameObject impactEffectPrefab;
    public AudioClip impactEffectClip;
}