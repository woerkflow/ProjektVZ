using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    
    public Type type;
    public GameObject spawnPrefab;
    public Transform[] spawnPoints;
    public float spawnTime;
    public float minSize;
    public float maxSize;

    public enum Type {
        Pumpkin,
        Chicken
    }
    
    [HideInInspector]
    public List<GameObject> spawns;
    
    private void Start() {
        InvokeRepeating(nameof(Spawn), 0f, spawnTime);
    }
    
    private void Spawn() {

        if (spawns.Count > 0) {
            spawns.RemoveAll(spawn => spawn == null);
        }
        
        if (spawns.Count >= spawnPoints.Length) {
            return;
        }
        Transform spawnPoint = spawnPoints[spawns.Count];
        GameObject obj = Instantiate(
            GetRandomSpawn(), 
            spawnPoint.position, 
            spawnPoint.rotation,
            gameObject.transform
        );
        
        // Add object to list
        spawns.Add(obj);
        
        // Set spawner as parent
        if (type == Type.Pumpkin) {
            obj.GetComponent<Mine>().SetParent(gameObject);
        }
    }

    private GameObject GetRandomSpawn() {
        GameObject spawn = spawnPrefab;
        float randomSize = Random.Range(minSize, maxSize);
        spawn.transform.localScale = new Vector3(
            randomSize, 
            randomSize, 
            randomSize
        );
        return spawn;
    }
}