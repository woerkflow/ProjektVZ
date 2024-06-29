using UnityEngine;

public class Building : MonoBehaviour {
    
    [Header("Building")]
    public int maxHealth;
    public GameObject destroyedBuildingPrefab;
    private int _currentHealth;
    
    #region Unity Methods

    private void Start() {
        _currentHealth = maxHealth;
    }
    
    #endregion
    
    #region Public methods

    public void SetHealth(int value) {
        _currentHealth = value;

        if (_currentHealth <= 0) {
            DestroyBuilding();
        }
    }

    public int GetHealth() {
        return _currentHealth;
    }
    
    #endregion
    
    #region Class methods

    private void DestroyBuilding() {
        Instantiate(destroyedBuildingPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    
    #endregion
}