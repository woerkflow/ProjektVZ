using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject spawnPrefab;
    public Transform[] spawnPoints;
    public float spawnTime;
    public float minSize;
    public float maxSize;

    private readonly List<ISpawnable> _spawns = new();
    private Coroutine _spawnCoroutine;
    private bool _isDestroyed;

    #region Unity Methods

    private void Start() {
        _isDestroyed = false;
        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }
    
    private void OnDestroy() {
        _isDestroyed = true;
        
        if (_spawnCoroutine != null) {
            StopCoroutine(_spawnCoroutine);
        }
    }

    #endregion

    #region Behaviour Methods

    private IEnumerator SpawnRoutine() {
        
        while (!_isDestroyed) {
            
            if (_spawns.Count < spawnPoints.Length) {
                CreateSpawn();
            }
            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void CreateSpawn() {
        Transform spawnPoint = spawnPoints[_spawns.Count];
        
        GameObject newSpawn = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation, transform);
        float randomSize = Random.Range(minSize, maxSize);
        newSpawn.transform.localScale = Vector3.one * randomSize;
        
        ISpawnable spawnableComponent = newSpawn.GetComponent<ISpawnable>();
        
        if (spawnableComponent != null) {
            spawnableComponent.SetParent(this);
        } else {
            Debug.LogWarning($"Spawned object does not have an ISpawnable component: {newSpawn.name}");
        }
    }

    #endregion
    
    #region Public Methods

    public void Register(ISpawnable spawnable) {
        _spawns.Add(spawnable);
    }

    public void Unregister(ISpawnable spawnable) {
        _spawns.Remove(spawnable);
    }
    
    #endregion
}