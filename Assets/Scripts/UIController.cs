using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Text Money;
    public Text Score;
    public Text Depth;
    public Text Cargo;
    public Image Fuel;
    public Image Vision;
    public Image FlashlightIcon;
    public Image FlashlightButton;
    public GameObject GameOverPanel;
    public GameObject VictoryPanel;
    public GameObject PausePanel;

    public Sprite FlashOff, FlashOn;

    private GameController ctl;
    private Player plr;
    private int lastMoney;
    private int lastScore;
    private UIScaler VisionScaler;

    private void Awake() {
        ctl = FindObjectOfType<GameController>();
        plr = FindObjectOfType<Player>();
        VisionScaler = Vision.GetComponent<UIScaler>();
    }

    private void Update() {
        if (lastScore < plr.Score) {
            int diff = Mathf.CeilToInt((plr.Score - lastScore) * 3.0f * Time.deltaTime);
            lastScore = Mathf.Min(lastScore + diff, plr.Score);
        }

        if (lastMoney < plr.Money) {
            int diff = Mathf.CeilToInt((plr.Money - lastMoney) * 3.0f * Time.deltaTime);
            lastMoney = Mathf.Min(lastMoney + diff, plr.Money);
        } else if (lastMoney > plr.Money) {
            int diff = Mathf.CeilToInt((lastMoney - plr.Money) * 3.0f * Time.deltaTime);
            lastMoney = Mathf.Max(lastMoney - diff, plr.Money);
        }

        Money.text = String.Format("${0:##,##0}", lastMoney);
        Score.text = String.Format("{0:##,##0} pts", lastScore);
        Cargo.text = String.Format("{0} / {1} cargo", plr.Cargo.Count, plr.CargoCapacity);

        // how tall is the pod? i dunno, some 3.5 meters?
        Depth.text = String.Format(CultureInfo.InvariantCulture, "{0:##,##0} ft.", (plr.transform.localPosition.y - 1.0f) * 18f);

        // fuel bar
        var rect = Fuel.rectTransform.sizeDelta;
        rect.x = plr.Fuel / plr.FuelMax * 500f;
        Fuel.rectTransform.sizeDelta = rect;

        bool flashlightShown = plr.Flashlight && !ctl.Paused; // no flashlight in pause, you sneaky bastard
        float visionScale = Mathf.Clamp01(-plr.transform.localPosition.y / 50f) * 2f;
        Color visionColor = Color.white;
        visionColor.a = Mathf.Clamp01(-plr.transform.localPosition.y / 24f);
        Vision.color = visionColor;
        VisionScaler.ScaleBuff = (3.0f - visionScale) * (flashlightShown ? 2f : 1f);

        FlashlightIcon.sprite = plr.Flashlight ? FlashOn : FlashOff;
        FlashlightIcon.GetComponent<ImagePulse>().Enabled = plr.Flashlight;
        //FlashlightButton.sprite = ctl.ControllerMode ? ButtonY : ButtonF;
        FlashlightButton.GetComponent<ImagePulse>().Enabled = !plr.Flashlight;

        PausePanel.SetActive(ctl.Paused);
        if (ctl.Paused) {
            if (Input.GetButtonDown("Confirm") || Input.GetButtonDown("Cancel"))
                ctl.Paused = false;
            else if (Input.GetButtonDown("Flashlight"))
                // this is on a weird button to help avoid accidental presses
                SceneManager.LoadScene("Title");
        } else if (!ctl.ShopOpen && !ctl.Victory && !ctl.GameOver && (Input.GetButtonDown("Cancel") || Input.GetKeyDown("joystick 1 button 7"))) {
            ctl.Paused = true;
        }

        if (ctl.GameOver) {
            GameOverPanel.SetActive(true);
            //GameOverOKIcon.sprite = ctl.ControllerMode ? ButtonA : ButtonEnter;

            if (Input.GetButtonDown("Cancel")) {
                SceneManager.LoadScene("Title");
            }
        }

        VictoryPanel.SetActive(ctl.Victory);
        if (ctl.Victory) {
            if (Input.GetButtonDown("Cancel")) {
                SceneManager.LoadScene("Title");
            } else if (Input.GetButtonDown("Confirm")) {
                // hide the panel and continue playing
                ctl.Victory = false;
            }
        }
    }

}
