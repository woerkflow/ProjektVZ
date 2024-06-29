using UnityEngine;

public class CameraController : MonoBehaviour {
    public float panSpeed;

    private void Update() {
        
        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(new Vector3(0f, 0f, panSpeed * Time.deltaTime), Space.World);
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(new Vector3(0f, 0f, -panSpeed * Time.deltaTime), Space.World);
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(new Vector3(-panSpeed * Time.deltaTime, 0f, 0f), Space.World);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(new Vector3(panSpeed * Time.deltaTime, 0f, 0f), Space.World);
        }
        Vector3 pos = transform.position;
        pos.y -= Input.mouseScrollDelta.y * panSpeed;
        transform.position = pos;
    }
}