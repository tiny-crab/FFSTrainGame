using UnityEngine;

public class Camera : MonoBehaviour {
    public Transform target;
    
    void Update() {
        transform.position = new Vector3 {
            x = target.position.x,
            y = transform.position.y,
            z = transform.position.z,
        };
    }
}
