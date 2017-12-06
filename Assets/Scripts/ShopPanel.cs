using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour {

    public GameObject PageFuel;
    public GameObject PageCargo;
    public GameObject PageUpgrade;

    public Text PageTitle;
    public Text FuelCostText;
    public Text CargoLeftText;
    public Text CargoRightText;
    public Text ConfirmText;
    public Text UpgradeText;
    public Image UpgradeArrow;

    private bool justChangedPage = false;
    private int currentPage = 0;
    private int currentUpgrade = 0;
    private Player plr;
    private GameController gctl;

    private static Color redColor = new Color(0.666f, 0.19f, 0.19f);
    private static Color blackColor = new Color(0.19f, 0.19f, 0.19f);

    private void Awake() {
        plr = FindObjectOfType<Player>();
        gctl = FindObjectOfType<GameController>();
    }

    private void OnEnable() {
        //if (plr == null || plr.Cargo == null) return;

        if (plr.Cargo.Count > 0)
            currentPage = 1;
        else if (plr.Fuel < plr.FuelMax - 0.5f)
            currentPage = 0;
        else {
            currentPage = 2;
        }
    }

    private void Update() {
        // controls for changing page
        float movement = Input.GetAxis("Horizontal");
        if (movement > 0.8f) {
            if (!justChangedPage)
                currentPage++;
            justChangedPage = true;
        } else if (movement < -0.8f) {
            if (!justChangedPage)
                currentPage--;
            justChangedPage = true;
        } else {
            justChangedPage = false;
        }
        if (currentPage < 0) currentPage = 2;
        if (currentPage > 2) currentPage = 0;

        PageFuel.SetActive(currentPage == 0);
        PageCargo.SetActive(currentPage == 1);
        PageUpgrade.SetActive(currentPage == 2);

        switch (currentPage) {
            case 0: // fuel
                PageTitle.text = "Refuel";

                float fuelMissing = plr.FuelMax - plr.Fuel;
                int fuelCost = Mathf.RoundToInt(fuelMissing * Tuning.COST_FUEL);
                if (fuelCost > plr.Money && plr.Money > 0) {
                    fuelMissing = plr.Money / Tuning.COST_FUEL;
                    fuelCost = plr.Money;

                    FuelCostText.text = String.Format("Partially refuel {1:0} L for ${0:##,##0}?", fuelCost, fuelMissing);
                    ConfirmText.color = blackColor;
                    ConfirmText.text = "Refuel";
                } else if (fuelMissing > 0.5f) {
                    FuelCostText.text = String.Format("Buy {1:0} L of fuel for ${0:##,##0}?", fuelCost, fuelMissing);
                    ConfirmText.color = fuelCost <= plr.Money ? blackColor : redColor;
                    ConfirmText.text = fuelCost <= plr.Money ? "Refuel" : "Can't Afford";
                } else {
                    FuelCostText.text = "Your fuel tank is full.";
                    ConfirmText.color = redColor;
                    ConfirmText.text = "N/A";
                }

                // Confirm key to get the fuel
                if (Input.GetButtonDown("Confirm") && fuelMissing > 0.5f) {
                    if (fuelCost <= plr.Money) {
                        plr.Fuel += fuelMissing;
                        plr.Money -= fuelCost;
                        gctl.SetShopOpen(false);
                    } else {
                        // cannot afford?
                        // TODO: buzzer noise or something
                    }
                }

                break;

            case 1: // cargo
                PageTitle.text = "Sell Cargo";

                bool victory = false;

                StringBuilder sbLeft = new StringBuilder();
                StringBuilder sbRight = new StringBuilder();
                int cargoValue = 0;
                for (CargoType cargoType = CargoType.Iron; cargoType <= CargoType.Katanium; cargoType++) {
                    var cargoTypeWrapper = cargoType;
                    int cargoCount = plr.Cargo.Count(thing => thing == cargoTypeWrapper);
                    if (cargoCount < 1) continue;

                    // player wins by selling an unobtainium
                    if (cargoType == CargoType.Unobtainium)
                        victory = true;
                    // total the value
                    int thisCargoValue = Tuning.GetMineralStats(cargoType).Value * cargoCount;
                    cargoValue += thisCargoValue;
                    sbLeft.AppendLine(cargoType.ToString() + ":");
                    if (cargoCount == 1)
                        sbRight.AppendFormat("${0:##,##0}", thisCargoValue);
                    else
                        sbRight.AppendFormat("{0} x ${1:##,##0} = ${2:##,##0}", cargoCount, Tuning.GetMineralStats(cargoType).Value, thisCargoValue);
                    sbRight.AppendLine();
                }

                if (cargoValue == 0) {
                    sbLeft.AppendLine("You have nothing to sell.");
                    ConfirmText.text = "N/A";
                } else {
                    sbLeft.AppendLine();
                    sbLeft.AppendFormat("Total: ${0:##,##0}", cargoValue);
                    ConfirmText.text = "Sell All";
                }

                CargoLeftText.text = sbLeft.ToString();
                CargoRightText.text = sbRight.ToString();
                ConfirmText.color = cargoValue > 0 ? blackColor : redColor;
                //String.Format("+${0:##,##0}  /  {1} items", cargoValue, plr.Cargo.Count);

                //int netValue = cargoValue - fuelCost;
                //TotalText.text = String.Format("= {0}${1:##,##0}", netValue < 0 ? "-" : "+", Mathf.Abs(netValue));
                //TotalText.color = netValue < 0 ? redColor : blackColor;

                //bool canAfford = plr.Money + netValue >= 0;

                if (Input.GetButtonDown("Confirm") && cargoValue > 0) {
                    plr.Money += cargoValue;
                    plr.Cargo.Clear();
                    gctl.SetShopOpen(false);

                    if (victory)
                        gctl.Victory = true;
                }
                break;

            case 2: // upgrades
                PageTitle.text = "Upgrades";

                float vertMove = Input.GetAxis("Vertical");
                if (vertMove > 0.8f)
                    currentUpgrade = 0;
                else if (vertMove < -0.8f)
                    currentUpgrade = 1;

                UpgradeArrow.rectTransform.anchoredPosition = new Vector2(45f, -105f - 100f * currentUpgrade);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Fuel Tank:");
                if (plr.LevelFuel < 7) {
                    sb.AppendFormat("${0:##,##0}  -  {1:0} L", Tuning.GetUpgradeCost(plr.LevelFuel + 1), Tuning.GetFuelCapacity(plr.LevelFuel + 1));
                    sb.AppendLine();
                } else {
                    sb.AppendLine("Maxed!");
                }
                sb.AppendLine();
                sb.AppendLine("Cargo Bay:");
                if (plr.LevelCargo < 7) {
                    sb.AppendFormat("${0:##,##0}  -  {1:0} slots", Tuning.GetUpgradeCost(plr.LevelCargo + 1), Tuning.GetCargoCapacity(plr.LevelCargo + 1));
                } else {
                    sb.AppendLine("Maxed!");
                }
                UpgradeText.text = sb.ToString();

                int currentLevel = currentUpgrade == 0 ? plr.LevelFuel : plr.LevelCargo;
                int cost = Tuning.GetUpgradeCost(currentLevel + 1);

                bool canBuy = cost <= plr.Money && currentLevel < 7;
                ConfirmText.color = cost <= plr.Money ? blackColor : redColor;
                ConfirmText.text = currentLevel < 7 ? (cost <= plr.Money ? "Purchase" : "Can't Afford") : "Maxed";

                if (Input.GetButtonDown("Confirm") && canBuy) {
                    plr.Money -= cost;
                    switch (currentUpgrade) {
                        case 0:
                            plr.LevelFuel++;
                            plr.FuelMax = Tuning.GetFuelCapacity(plr.LevelFuel);
                            plr.Fuel = plr.FuelMax;
                            break;
                        case 1:
                            plr.LevelCargo++;
                            plr.CargoCapacity = Tuning.GetCargoCapacity(plr.LevelCargo);
                            break;
                        default:
                            Debug.LogError("bad currentUpgrade");
                            break;
                    }

                    gctl.SetShopOpen(false);
                }

                break;
        }


        if (Input.GetButtonDown("Cancel")) {
            gctl.SetShopOpen(false);
            Input.ResetInputAxes();
        }
    }

}
