using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour {

    public Image background;
    public Image fader;
    public GameObject loadingText;

    private bool fading = false;
    private float fadeOpacity = 0f;
    private float movement;

    private void Start() {
        background.rectTransform.sizeDelta = new Vector2(Screen.width, Mathf.Max(Screen.height * 2, 1280f * 2f));
    }

    private void Update() {
        Vector2 pos1 = background.rectTransform.offsetMin;
        movement += Time.deltaTime * 64f;
        pos1.y += Time.deltaTime * 64f;
        if (movement >= 1280f) {
            movement -= 1280f;
            pos1.y -= 1280f;
        }

        background.rectTransform.offsetMin = pos1;

        if (fading) {
            fadeOpacity += Time.deltaTime;
            fader.color = new Color(0, 0, 0, fadeOpacity);
            if (fadeOpacity >= 1f) {
                loadingText.SetActive(true);
                SceneManager.LoadScene("Game");
            }
            return;
        }

        if (Input.GetButtonDown("Confirm")) {
            fading = true;
        } else if (Input.GetButtonDown("Cancel")) {
            Application.Quit();
        }
    }

}
