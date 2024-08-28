using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour {

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Material[] materials;

    private Coroutine _updateMaterialCoroutine;
    private int _materialIndex;
    private bool _isDestroyed;
    
    private void Start() {
        _isDestroyed = false;
        _materialIndex = 0;
        _updateMaterialCoroutine = StartCoroutine(UpdateMaterialRoutine());
    }

    private void OnDestroy() {
        _isDestroyed = true;

        if (_updateMaterialCoroutine != null) {
            StopCoroutine(_updateMaterialCoroutine);
        }
    }

    private void UpdateMaterial() {
        lineRenderer.SetMaterials(new List<Material> { materials[_materialIndex] });
        _materialIndex = _materialIndex >= materials.Length
            ? 0
            : _materialIndex + 1;
    }
    
    private IEnumerator UpdateMaterialRoutine() {
        
        while (!_isDestroyed) {
            yield return new WaitForSeconds(0.1f);
            UpdateMaterial();
        }
    }
}
