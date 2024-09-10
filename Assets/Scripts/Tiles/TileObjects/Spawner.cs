using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    [SerializeField] private SpawnerBlueprint blueprint;

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
            
            if (_spawns.Count < blueprint.spawnPoints.Length) {
                CreateSpawn();
            }
            yield return new WaitForSeconds(blueprint.spawnTime);
        }
    }

    private void CreateSpawn() {
        Transform spawnPoint = blueprint.spawnPoints[_spawns.Count];
        
        GameObject newSpawn = Instantiate(blueprint.spawnPrefab, spawnPoint.position, spawnPoint.rotation, transform);
        float randomSize = Random.Range(blueprint.minSize, blueprint.maxSize);
        newSpawn.transform.localScale = Vector3.one * randomSize;
        
        ISpawnable spawnableComponent = newSpawn.GetComponent<ISpawnable>();
        spawnableComponent?.SetParent(this);
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