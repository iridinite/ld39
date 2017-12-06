using System;
using UnityEngine;

public enum TileType {
    Barrier,
    Stone,
    Iron,
    Copper,
    Tin,
    Silver,
    Gold,
    Platinum,
    Emerald,
    Ruby,
    Diamond,
    Adamantium,
    Unobtainium,
    Artifact,
    Fossil,
    Oil,
    Boulder
}

public enum CargoType {
    Invalid,
    Iron,
    Copper,
    Tin,
    Silver,
    Gold,
    Platinum,
    Emerald,
    Ruby,
    Diamond,
    Adamantium,
    Unobtainium,
    Bronze,
    Oragnium,
    Katanium
}

public class Tile : MonoBehaviour {

    public TileType type;
    public Material[] materials;
    public GameObject pickupText;

    private static bool mergeTutorialShown = false;

    private void Start() {
        GetComponent<Renderer>().material = materials[(int)type];
    }

    private void DoFloatyText(string text) {
        GameObject go = Instantiate(pickupText);
        go.transform.position = this.transform.position + Vector3.up * 0.45f;

        TextMesh textmesh = go.GetComponent<TextMesh>();
        textmesh.text = text;
    }

    public void PickUp() {
        Player plr = FindObjectOfType<Player>();

        CargoType cargo = GetCargoType(type);
        if (cargo == CargoType.Invalid) {
            switch (type) {
                case TileType.Artifact:
                    plr.Score += 10000;
                    plr.Money += 5000;
                    DoFloatyText("Ancient Artifact: $5,000");
                    return;
                case TileType.Fossil:
                    plr.Score += 2500;
                    plr.Money += 1000;
                    DoFloatyText("Fossil: $1,000");
                    return;
                case TileType.Oil:
                    plr.Score += 2500;
                    plr.Fuel = Math.Min(plr.Fuel + 20f, plr.FuelMax);
                    DoFloatyText("Oil: Restored Fuel");
                    return;
                default:
                    // stone, probably
                    plr.Score += 25;
                    return;
            }
        }

        if (cargo == CargoType.Unobtainium)
            plr.Score += 20000;
        else
            plr.Score += Tuning.GetMineralStats(cargo).Value * 4;

        // specify mergeable minerals
        CargoType mergeRequire = CargoType.Invalid;
        CargoType mergeResult = CargoType.Invalid;
        if (cargo == CargoType.Tin) {
            mergeRequire = CargoType.Copper;
            mergeResult = CargoType.Bronze;
        }
        if (cargo == CargoType.Copper) {
            mergeRequire = CargoType.Tin;
            mergeResult = CargoType.Bronze;
        }
        if (cargo == CargoType.Iron) {
            mergeRequire = CargoType.Platinum;
            mergeResult = CargoType.Oragnium;
        }
        if (cargo == CargoType.Platinum) {
            mergeRequire = CargoType.Iron;
            mergeResult = CargoType.Oragnium;
        }
        if (cargo == CargoType.Adamantium) {
            mergeRequire = CargoType.Gold;
            mergeResult = CargoType.Katanium;
        }
        if (cargo == CargoType.Gold) {
            mergeRequire = CargoType.Adamantium;
            mergeResult = CargoType.Katanium;
        }

        // attempt to merge two cargo items
        if (mergeRequire != CargoType.Invalid) {
            int index = plr.Cargo.IndexOf(mergeRequire);
            if (index != -1) {
                if (!mergeTutorialShown) {
                    // tutorial popup for combining minerals
                    mergeTutorialShown = true;
                    TutorialManager.QueueTutorial(3);
                }

                DoFloatyText(String.Format("{0} + {1} = {2}", cargo, mergeRequire, mergeResult));
                plr.Cargo[index] = mergeResult; // replace the existing slot with the merged type
                return;
            }
        }

        if (plr.Cargo.Count >= plr.CargoCapacity && cargo != CargoType.Unobtainium)
            DoFloatyText("CARGO FULL");
        else {
            DoFloatyText(type.ToString());
            plr.Cargo.Add(cargo);
        }
    }

    public static CargoType GetCargoType(TileType type) {
        switch (type) {
            case TileType.Iron:
                return CargoType.Iron;
            case TileType.Copper:
                return CargoType.Copper;
            case TileType.Tin:
                return CargoType.Tin;
            case TileType.Silver:
                return CargoType.Silver;
            case TileType.Gold:
                return CargoType.Gold;
            case TileType.Platinum:
                return CargoType.Platinum;
            case TileType.Emerald:
                return CargoType.Emerald;
            case TileType.Ruby:
                return CargoType.Ruby;
            case TileType.Diamond:
                return CargoType.Diamond;
            case TileType.Adamantium:
                return CargoType.Adamantium;
            case TileType.Unobtainium:
                return CargoType.Unobtainium;
            default:
                return CargoType.Invalid;
        }
    }

}
