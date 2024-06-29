using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    
    public GameObject[] spawnPrefabs;
    public Transform[] spawnPoints;
    public float spawnTime;
    public List<GameObject> spawns;
    
    private void Start() {
        spawns = new List<GameObject>();
        InvokeRepeating(nameof(Spawn), 0f, spawnTime);
    }
    
    private void Spawn() {

        if (spawns.Count > 0) {
            spawns.RemoveAll(spawn => spawn == null);
        }
        
        if (spawns.Count >= spawnPoints.Length) {
            return;
        }
        GameObject spawn = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
        Transform spawnPoint = spawnPoints[spawns.Count];
        GameObject obj = Instantiate(spawn, spawnPoint.position, spawnPoint.rotation);
        spawns.Add(obj);
    }
}