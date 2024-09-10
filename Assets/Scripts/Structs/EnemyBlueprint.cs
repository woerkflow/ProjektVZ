using UnityEngine;

[System.Serializable]
public struct EnemyBlueprint {
    public GameObject mainTarget;
    public float speed;
    public float attackSpeed;
    public int minDamage;
    public int maxDamage;
    public CapsuleCollider capsuleCollider;
    public Animator animator;
    public string walkParameter;
    public string attackParameter;
    public string dieParameter;
    public int maxHealth;
    public float deadTime;
    public int lootAmount;
    public Loot[] loots;
}