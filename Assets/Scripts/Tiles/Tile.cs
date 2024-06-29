using System;
using UnityEngine;

public class Tile : MonoBehaviour {
    
    [Header("Tile")]
    public GameObject selectEffect;
    public Material selectColor;
    public Material standardColor;
    public Renderer rend;
    public BuildingBlueprint building;

    #region Unity Methods

    public void OnMouseEnter() {
        rend.material = selectColor;
        selectEffect.SetActive(true);
    }

    public void OnMouseDown() {
        Instantiate(building.prefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void OnMouseExit() {
        rend.material = standardColor;
        selectEffect.SetActive(false);
    }

    #endregion
}