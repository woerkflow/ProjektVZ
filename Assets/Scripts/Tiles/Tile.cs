using UnityEngine;

public class Tile : MonoBehaviour {
    
    [Header("Tile")]
    public GameObject selectEffect;
    public BuildingBlueprint building;

    #region Unity Methods

    public void OnMouseEnter() {
        selectEffect.SetActive(true);
    }

    public void OnMouseDown() {
        Instantiate(building.prefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void OnMouseExit() {
        selectEffect.SetActive(false);
    }

    #endregion
}