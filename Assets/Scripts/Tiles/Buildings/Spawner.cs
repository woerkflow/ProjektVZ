using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
    
    public Type type;
    public GameObject spawnPrefab;
    public Transform[] spawnPoints;
    public float spawnTime;
    public float minSize;
    public float maxSize;

    public enum Type {
        Pumpkin,
        Chicken,
        Bull,
    }
    
    [HideInInspector]
    public List<GameObject> spawns;
    
    
    #region Unity methods
    
    private void Start() {
        InvokeRepeating(nameof(Spawn), 0f, spawnTime);
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void Spawn() {

        // Cleanse list
        if (spawns.Count > 0) {
            spawns.RemoveAll(spawn => spawn == null);
        }
        
        if (spawns.Count >= spawnPoints.Length) {
            return;
        }
        CreateSpawn();
    }

    private void CreateSpawn() {
        Transform spawnPoint = spawnPoints[spawns.Count];
        GameObject obj = Instantiate(
            GetRandomSpawn(spawnPrefab, minSize, maxSize), 
            spawnPoint.position, 
            spawnPoint.rotation,
            gameObject.transform
        );
        
        // Add object to list
        spawns.Add(obj);
        
        // Set spawner as parent
        switch (type) {
            case Type.Pumpkin: 
                obj.GetComponent<Mine>().SetParent(gameObject);
                break;
            case Type.Chicken:
                obj.GetComponent<Spawn>().SetParent(gameObject);
                break;
            case Type.Bull:
                obj.GetComponent<Spawn>().SetParent(gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static GameObject GetRandomSpawn(GameObject spawnPrefab, float minSize, float maxSize) {
        GameObject spawn = spawnPrefab;
        float randomSize = Random.Range(minSize, maxSize);
        spawn.transform.localScale = new Vector3(
            randomSize, 
            randomSize, 
            randomSize
        );
        return spawn;
    }
    
    #endregion
}