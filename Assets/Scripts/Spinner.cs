using UnityEngine;

public class Spinner : MonoBehaviour {

    public Vector3 Axis = Vector3.up;
    public float Speed = 8f;

    private float time;

    private void Update() {
        transform.localEulerAngles += Axis * Speed * Time.deltaTime;
    }

}
