using UnityEngine;
using UnityEngine.UI;

public class ButtonIcon : MonoBehaviour {

    public Sprite iconKey, iconGamepad;

    private GameController ctl;
    private Image img;

    private void Awake() {
        ctl = FindObjectOfType<GameController>();
        img = GetComponent<Image>();
    }

    private void Update() {
        img.sprite = ctl.ControllerMode ? iconGamepad : iconKey;
    }

}
