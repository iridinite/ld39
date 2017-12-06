using UnityEngine;

public class DelayDelete : MonoBehaviour {

    public float delay;

    private void Start() {
        Destroy(gameObject, delay);
    }

}
