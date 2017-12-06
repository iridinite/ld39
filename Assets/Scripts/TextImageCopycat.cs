using UnityEngine;
using UnityEngine.UI;

public class TextImageCopycat : MonoBehaviour {

    public Text master;
    private Text self;

	private void Start() {
	    self = GetComponent<Text>();
	}

	private void Update() {
	    self.text = master.text;
	}

}
