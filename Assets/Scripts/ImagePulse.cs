using UnityEngine;

public class ImagePulse : MonoBehaviour {

    public float Magnitude = 0.1f;
    public float Speed = 2.0f;

    private RectTransform rect;
    private Vector2 baseSize;
    private float time;

    public bool Enabled { get; set; }

    private void Start() {
        rect = GetComponent<RectTransform>();
        baseSize = rect.sizeDelta;
        Enabled = true;
    }

    private void Update() {
        if (!Enabled) {
            rect.sizeDelta = baseSize;
            return;
        }

        time += Time.deltaTime;

        Vector2 size = baseSize * (1.0f + Mathf.Sin(time * Speed) * Magnitude + Magnitude);
        rect.sizeDelta = size;
    }

}
