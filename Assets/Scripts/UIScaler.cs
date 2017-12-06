using UnityEngine;

public class UIScaler : MonoBehaviour {

    public float ScaleBuff { get; set; }

    private void Start() {
        ScaleBuff = 1f;
    }

    private void Update() {
        RectTransform canvas = this.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        RectTransform myRect = GetComponent<RectTransform>();

        float size = Mathf.Max(canvas.sizeDelta.x, canvas.sizeDelta.y, canvas.sizeDelta.y * 1.777778f);
        myRect.sizeDelta = new Vector2(size, size) * ScaleBuff;
    }

}
