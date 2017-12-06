using UnityEngine;

public class TextMeshCopycat : MonoBehaviour {

    public TextMesh master;
    private TextMesh self;

	private void Awake() {
	    self = GetComponent<TextMesh>();
	}

	private void Update() {
	    self.text = master.text;
	}

}
