using UnityEngine;

[System.Serializable]
public struct SpawnerBlueprint {
    public GameObject spawnPrefab;
    public Transform[] spawnPoints;
    public float spawnTime;
    public float minSize;
    public float maxSize;
}