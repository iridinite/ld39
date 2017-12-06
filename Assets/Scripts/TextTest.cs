using UnityEngine;

public class TextTest : MonoBehaviour {

    public bool DoMove = true;
    public bool IsOutline = false;

    private float lifetime;
    private TextMesh text;

	private void Start() {
	    lifetime = 0.0f;
	    text = GetComponent<TextMesh>();
	}

	private void Update() {
	    lifetime += Time.deltaTime;
        if (!IsOutline)
	        transform.localScale = Vector3.one * (2.0f - Mathf.Clamp01(lifetime * 4.0f));

        if (DoMove)
	        transform.localPosition += Vector3.up * Time.deltaTime * 0.35f;

	    Color textColor = IsOutline ? Color.black : Color.white;
	    textColor.a = Mathf.Clamp01(2.0f - lifetime);
	    text.color = textColor;

        if (lifetime >= 2.0f)
            Destroy(gameObject);
	}

}
