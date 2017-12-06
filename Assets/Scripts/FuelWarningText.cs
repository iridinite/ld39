using System;
using UnityEngine;
using UnityEngine.UI;

public class FuelWarningText : MonoBehaviour {

    public Text copycat;
    public Color color1, color2;

    private Text text;
    private Player plr;

    private bool fiftyPercentShown;
    private float fiftyPercentDuration;

    private bool blinkMode;
    private float blinkTime;

    private const float blinkTimeMax = 0.8f;

    private void Awake() {
        text = GetComponent<Text>();
        //copycat = text.transform.GetChild(0).GetComponent<Text>(); // what do you mean hacky code
        plr = FindObjectOfType<Player>();
        fiftyPercentShown = false;
    }

    private void Update() {
        float ratio = plr.Fuel / plr.FuelMax;
        if (ratio >= 0.9f)
            fiftyPercentShown = false;

        if (ratio <= 0.5f && !fiftyPercentShown) {
            fiftyPercentShown = true;
            fiftyPercentDuration = 4f;
            text.text = "50% remaining";
        }

        if (ratio < 0.1f)
            text.text = "FUEL CRITICAL";
        else if (ratio < 0.25f)
            text.text = "FUEL LOW";

        bool shouldBlink = fiftyPercentDuration > 0f || ratio < 0.25f;
        if (fiftyPercentDuration > 0f)
            fiftyPercentDuration -= Time.deltaTime;

        if (shouldBlink) {
            blinkTime += Time.deltaTime;
            if (blinkTime >= blinkTimeMax) {
                blinkTime = 0f;
                blinkMode = !blinkMode;
            }
            copycat.color = blinkMode
                ? Color.Lerp(color1, color2, blinkTime / blinkTimeMax)
                : Color.Lerp(color2, color1, blinkTime / blinkTimeMax);
        } else {
            text.text = String.Empty;
        }
    }

}
