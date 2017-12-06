using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    public Sprite[] TutorialImages;

    private Image image;
    private static readonly Queue<int> tutorialQueue = new Queue<int>();
    private int nowShowing;
    private int state;
    private float opacity;
    private float waitTime;

    private void Awake() {
        image = GetComponent<Image>();
    }

    public static void QueueTutorial(int image) {
        tutorialQueue.Enqueue(image);
    }

    private void Update() {
        switch (state) {
            case 0:
                if (tutorialQueue.Count == 0) return;
                nowShowing = tutorialQueue.Dequeue();
                image.sprite = TutorialImages[nowShowing];
                state = 1;
                break;
            case 1:
                opacity += Time.deltaTime;
                image.color = new Color(1f, 1f, 1f, opacity);
                if (opacity >= 1f) {
                    waitTime = 6f;
                    state = 2;
                }
                break;
            case 2:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0f) state = 3;
                break;
            case 3:
                opacity -= Time.deltaTime;
                image.color = new Color(1f, 1f, 1f, opacity);
                if (opacity <= 0f) state = 0;
                break;
        }
    }

}
